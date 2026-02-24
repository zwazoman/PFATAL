using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _scripts.PlayerCharacter;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Le server doit appeler TriggerGameStart,
/// et peut récupérer l'event OnGameEnded pour afficher le résultat de la partie.
/// Il y'a également une méthode GetScoreBoard().
/// </summary>
public abstract class GameRulesBase
{
    public bool IsPlaying = false;
    public event Action OnGameStarted;
    public event Action<GameResult> OnGameEnded;
    
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
    
    public class GameResult
    {
        /// <summary>
        /// ClientID, Player ScoreEntry
        /// </summary>
        public SortedSet<ScoreEntry> ScoreBoard;
        //...
    }
    
    public struct ScoreEntry :IComparable<ScoreEntry>
    {
        public string PlayerName;
        public ulong ClientID;
        public int Rank,Kills,Deaths,Points;
        public int CompareTo(ScoreEntry other)
        {
            return other.Points.CompareTo(Points);
        }
    }
    
    //score
    
    protected void UpdatePlayersRanks()
    {
        SortedSet<ScoreEntry> scores = new SortedSet<ScoreEntry>();
        foreach (var player in _players.Values)
        {
            scores.Add(player.Score);
        }

        int i = 0;
        foreach (var rank in scores.ToArray())
        {
            _players[rank.ClientID].Score.Rank = i++;
        }
    }

    /// <summary>
    /// retourne le classement des joeurs trié par rank.
    /// </summary>
    public SortedSet<ScoreEntry> GetScoreBoard()
    {
        SortedSet<ScoreEntry> scoreBoard = new();

        foreach (PlayerData player in _players.Values)
        {
            scoreBoard.Add(player.Score);
        }
        
        return scoreBoard;
    }
    
    // == game flow ==
    
    public GameRulesBase(List<ulong> clientIDs)
    {
        if(!NetworkManager.Singleton.IsServer) 
            throw new Exception("Seul le server peut gérer les regles du jeu.");
            
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
        OnGameStarted?.Invoke();
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
