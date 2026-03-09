using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using _Scripts.Exceptions;
using _scripts.PlayerCharacter;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
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
    protected Dictionary<ulong,PlayerData> _players = new ();
    
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
                + ", character : "+Character
                + ", score : "+Score;
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
    

    
    //score
    
    protected void UpdateScoreBoard()
    {
        LeaderBoardData leaderBoard = GetLeaderBoard();
        OnScoreBoardUpdated?.Invoke(leaderBoard);
    }

    /// <summary>
    /// retourne le classement des joeurs trié par rank.
    /// </summary>
    public LeaderBoardData GetLeaderBoard()
    {
        LeaderBoardData leaderBoard = new();
        string s = "";
        s+=("= Get leader board.=");
        s+=("   Player count : " + _players.Count.ToString());
        
        SortedSet<ScoreEntry> sortedEntries = new();
        foreach (PlayerData player in _players.Values)
        {
            bool success = sortedEntries.Add(player.Score);
            s+=("      adding score entry into temp set from player : "+player+". Success : "+success);
        }
        s+=("   temp set count : "+sortedEntries.Count);

        int i = 0;
        foreach (var entry in sortedEntries)
        {
            bool success = leaderBoard.entries.Add(new ScoreEntry(
                entry.ClientID,
                ++i,
                entry.Kills,
                entry.Deaths,
                entry.Points));
            
            s+=("      adding score entry into result set : "+entry+". Success : "+success);
        }
        s+=("   added player scores in the result sorted set.");
        s+=("   Set count : "+leaderBoard.entries.Count);
        Debug.Log(s);
        
        return leaderBoard;
    }
    
    // == game flow ==
    
    public GameRulesBase(List<ulong> clientIDs)
    {
        Debug.Log($"instantiated game rule.");
        
        if(!NetworkManager.Singleton.IsServer) 
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
        if(IsPlaying) throw new Exception("Game has started already.");
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
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        
    }

    /// <summary>
    /// doit être appelé par le GameMode quand
    /// il veut que la partie se termine.
    /// </summary>
    protected void TriggerGameEnd()
    {
        IsPlaying = false;
        GameResult result = EndGame();
        OnGameEnded?.Invoke(result);
    }

    //abstract functions
    
    /// <summary>
    /// called before the game starts
    /// </summary>
    protected abstract void StartGame();
    
    /// <summary>
    /// called after the game ends.
    /// </summary>
    protected abstract GameResult EndGame();
    

}
