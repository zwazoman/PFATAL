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
    }
    
    public class GameResult : INetworkSerializable
    {
        /// <summary>
        /// ClientID, Player ScoreEntry
        /// </summary>
        public LeaderBoardData LeaderBoard;

        public override string ToString()
        {
            const string space = " | ";
            string s = "";
            foreach (var entry in LeaderBoard.entries)
            {
                s +=
                    "Player : " + entry.ClientID + space +
                    "Kills : " + entry.Kills + space +
                    "Deaths : " + entry.Deaths + space +
                    "Points : " + entry.Points + space +
                    "Rank : " + entry.Rank + '\n';
            }

            return s;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            throw new NotImplementedException();
        }
    }
    
    public struct ScoreEntry :IComparable<ScoreEntry>, INetworkSerializeByMemcpy
    {
        public ulong ClientID;
        public int Rank,Kills,Deaths,Points;
        public int CompareTo(ScoreEntry other)
        {
            return other.Points.CompareTo(Points);
        }
    }
    
    //score
    
    protected void UpdateScoreBoard()
    {
        LeaderBoardData leaderBoard = new();
        foreach (var player in _players.Values)
        {
            leaderBoard.entries.Add(player.Score);
        }

        int i = 0;
        foreach (var rank in leaderBoard.entries)
        {
            _players[rank.ClientID].Score.Rank = i++;
        }
        
        OnScoreBoardUpdated?.Invoke(leaderBoard);
    }

    /// <summary>
    /// retourne le classement des joeurs trié par rank.
    /// </summary>
    public LeaderBoardData GetLeaderBoard()
    {
        LeaderBoardData leaderBoard = new();

        foreach (PlayerData player in _players.Values)
        {
            leaderBoard.entries.Add(player.Score);
        }
        
        return leaderBoard;
    }
    
    // == game flow ==
    
    public GameRulesBase(List<ulong> clientIDs)
    {
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
    public async void TriggerGameStart()
    {
        if(IsPlaying) throw new Exception("Game has started already.");

        IsPlaying = true;
        await SpawnPlayerCharacters();
        _gameStartTime = TimeStamp.Now;
        StartGame();
        OnGameStarted?.Invoke(TimeStamp.Now);
    }

    private async Task SpawnPlayerCharacters()
    {
        foreach (PlayerData playerData in _players.Values)
        {
            playerData.Character = await PlayerSpawner.Instance.SpawnPlayerCharacter(playerData.ClientID);
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
