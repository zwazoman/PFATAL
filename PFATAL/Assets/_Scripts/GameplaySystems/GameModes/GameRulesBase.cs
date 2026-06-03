using System;
using System.Collections.Generic;
using _Scripts.Exceptions;
using _scripts.PlayerCharacter;
using JetBrains.Annotations;
using NetworkTime;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Le server doit appeler TriggerGameStart,
/// et peut récupérer l'event OnGameEnded pour afficher le résultat de la partie.
/// Il y'a également une méthode GetLeaderBoard().
/// </summary>
public abstract class GameRulesBase
{
    public bool IsPlaying = false;
    public event Action<float> OnGameStarted;
    public event Action<GameResult> OnGameEnded;
    public event Action<LeaderBoardData> OnScoreBoardUpdated; 
    
    private float _gameStartTime;
    public float TimeSinceGameStart => TimeStamp.Now - _gameStartTime;

    /// <summary>
    /// client id, player data
    /// </summary>
    protected Dictionary<ulong, PlayerData> _players = new();
    
    // == data ==
    protected class PlayerData
    {
        public ulong ClientID;
        [CanBeNull] public PlayerCharacter Character;
        public ScoreEntry Score;

        public override string ToString()
        {
            return 
                "client id : " + ClientID
                + ", character : " + Character
                + ", score : " + Score;
        }
    }
    
    public class GameResult : INetworkSerializable
    {
        /// <summary>
        /// ClientID, Player ScoreEntry
        /// </summary>
        public LeaderBoardData LeaderBoard;

        public override string ToString()
        {
            string s = "";
            s += LeaderBoard.ToString();
            return s;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeNetworkSerializable(ref LeaderBoard);
        }
    }

    // == score ==
    
    protected void UpdateScoreBoard()
    {
        LeaderBoardData leaderBoard = GetLeaderBoard();
        OnScoreBoardUpdated?.Invoke(leaderBoard);
    }

    /// <summary>
    /// Retourne le classement des joueurs triés par rank.
    /// FIXED: utilise une List au lieu d'un SortedSet pour éviter
    /// la déduplication silencieuse des joueurs avec des scores égaux.
    /// </summary>
    public LeaderBoardData GetLeaderBoard()
    {
        LeaderBoardData leaderBoard = new();
        string s = "";
        s += "= Get leader board. =";
        s += "   Player count : " + _players.Count;

        // ✅ FIX : List au lieu de SortedSet
        // SortedSet déduplique via CompareTo() — si deux joueurs ont
        // les mêmes stats, le second est silencieusement rejeté.
        List<ScoreEntry> tempList = new();
        foreach (PlayerData player in _players.Values)
        {
            tempList.Add(player.Score);
            s += "      added score entry from player : " + player;
        }

        tempList.Sort(); // utilise CompareTo de ScoreEntry, sans déduplication
        s += "   temp list count : " + tempList.Count;

        int i = 0;
        foreach (var entry in tempList)
        {
            ScoreEntry ranked = new ScoreEntry(
                entry.ClientID,
                ++i,
                entry.Kills,
                entry.Deaths,
                entry.Points);

            leaderBoard.entries.Add(ranked);
            s += "      added ranked entry : " + entry;
        }

        s += "   Final leaderboard count : " + leaderBoard.entries.Count;
        Debug.Log(s);
        
        return leaderBoard;
    }
    
    // == game flow ==
    
    public GameRulesBase(List<ulong> clientIDs)
    {
        Debug.Log($"instantiated game rule.");
        
        if (!NetworkManager.Singleton.IsServer) 
            throw new NetworkAuthorityException("Seul le server peut gérer les regles du jeu.");
        
        foreach (ulong clientID in clientIDs)
        {
            _players.Add(clientID, new PlayerData() { ClientID = clientID });
            _players[clientID].Score = new ScoreEntry() { ClientID = clientID };
        }       
    }

    /// <summary>
    /// Déclenche le spawn des joueurs et le début de la partie.
    /// Doit être appelé par le server après que tous
    /// les clients se soient connectés.
    /// </summary>
    public async Awaitable TriggerGameStart()
    {
        if (IsPlaying) throw new Exception("Game has started already.");
        Debug.Log("game rule trigger game start");

        IsPlaying = true;
        await SpawnPlayerCharacters();
        Debug.Log("spawned all players.");
        
        _gameStartTime = TimeStamp.Now;
        
        StartGame();
        OnGameStarted?.Invoke(_gameStartTime);
        UpdateScoreBoard();
    }

    private async Awaitable SpawnPlayerCharacters()
    {
        try
        {
            foreach (PlayerData playerData in _players.Values)
            {
                playerData.Character =
                    await PlayerCharacterSpawner.Instance.SpawnInitialPlayerCharacter(playerData.ClientID);

                var capturedPlayer = playerData;

                capturedPlayer.Character.health.OnDie += () =>
                {
                    RegisterDeath(capturedPlayer);
                };
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public PlayerCharacter GetPlayerCharacter(ulong playerClientId)
    {
        if (!_players.ContainsKey(playerClientId))
        {
            Debug.LogError("wrong player client ID");
            return null;
        }

        return _players[playerClientId].Character;
    }

    /// <summary>
    /// Doit être appelé par le GameMode quand
    /// il veut que la partie se termine.
    /// </summary>
    protected void TriggerGameEnd()
    {
        IsPlaying = false;
        GameResult result = EndGame();
        OnGameEnded?.Invoke(result);
    }

    protected void RegisterDeath(PlayerData player)
    {
        ulong victimClientID = player.ClientID;
        ulong killerClientID = player.Character.health.LastDamageSourceClientID;

        PermanentPlayerIdentity victimIdentity = GameManager.GetPlayerIdentity(victimClientID);
        PermanentPlayerIdentity killerIdentity = GameManager.GetPlayerIdentity(killerClientID);

        ulong victimSteamID = ulong.Parse(victimIdentity.platformID);
        ulong killerSteamID = ulong.Parse(killerIdentity.platformID);

        int weaponID = player.Character.health.LastDamageWeaponID;
        float timeOfDeath = GameManager.Instance.TimeSinceGameStart;
        float distance = Vector3.Distance(
            player.Character.transform.position,
            player.Character.health.SourcePos
        );

        DataCollector.Instance.RegisterDeath(victimSteamID, killerSteamID, weaponID, distance, timeOfDeath);
    }

    // == abstract ==
    
    /// <summary>
    /// Called before the game starts.
    /// </summary>
    protected abstract void StartGame();
    
    /// <summary>
    /// Called after the game ends.
    /// </summary>
    protected abstract GameResult EndGame();
}