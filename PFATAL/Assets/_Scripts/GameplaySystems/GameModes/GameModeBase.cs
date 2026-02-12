using System;
using System.Collections.Generic;
using _scripts.PlayerCharacter;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public abstract class GameModeBase
{
    public bool IsPlaying = false;
    public event Action OnGameStarted;
    public event Action<GameResult> OnGameEnded;

    
    protected List<Player> _players;
    
    // == data ==
    protected struct Player
    {
        public int ClientID;
        public string Name;
        [CanBeNull] public PlayerCharacter Character;
        public ScoreEntry Score;
    }
    
    public struct GameResult
    {
        /// <summary>
        /// ClientID, Player ScoreEntry
        /// </summary>
        public Dictionary<int, ScoreEntry> ScoreBoard;
        
        //...
    }
    
    public struct ScoreEntry
    {
        public string PlayerName;
        public int ClientID;
        public int Rank,Kills,Deaths;
    }
    
    // == game flow ==
    
    public virtual void Init(List<int> clientIDs)
    {
        foreach (int clientID in clientIDs)
        {
            _players.Add(new Player() { ClientID = clientID });
        }           
    }

    public void TriggerGameStart()
    {
        IsPlaying = true;
        StartGame();
        OnGameStarted?.Invoke();
    }
    
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
    /// called after the game ends
    /// </summary>
    protected abstract GameResult EndGame();
    

}
