using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    //todo : scriptable object avec game settings ?
    private const float DEATH_MATCH_GAME_DURATION = 100;
    
    public enum GameMode
    {
        DeathMatch,
        None
    }

    private GameRulesBase serverGameRules;
    
    //synced events
    public event Action OnGameStarted;
    public event Action<GameRulesBase.GameResult> OnGameEnded;
    
    //synced variables
    public bool IsPlaying { get; private set ; } = false;
    public float TimeSinceGameStart => TimeStamp.Now - _startTime;
    
    public LeaderBoardData LeaderBoard;

    private float _startTime;

    //todo : relier au lobby et au game rule
    
    public void StartGame(List<ulong> clientIDs,GameMode gameMode)
    {
        if (IsServer)
        {
            switch (gameMode)
            {
                case GameMode.DeathMatch:
                    serverGameRules = new GameRulesDeathMatch(clientIDs,DEATH_MATCH_GAME_DURATION);
                    break;
                default:
                    throw new Exception("Game Mode not set");
                    break;
            };

            serverGameRules.OnGameStarted += OnServerStartGameRPC;
            serverGameRules.OnGameEnded += OnServerEndGameRPC;
            serverGameRules.OnScoreBoardUpdated += OnScoreBoardUpdatedRpc;
            
            serverGameRules.TriggerGameStart();
        }
    }

    
//sync RPCs

    [Rpc(SendTo.Everyone)]
    void OnScoreBoardUpdatedRpc(LeaderBoardData newLeaderboard)
    {
        LeaderBoard = newLeaderboard;
    }
    
    [Rpc(SendTo.Everyone)]
    void OnServerStartGameRPC(float startTime)
    {
        IsPlaying = true;
        _startTime = startTime;
        OnGameStarted?.Invoke();
    }
    
    [Rpc(SendTo.Everyone)]
    void OnServerEndGameRPC(GameRulesBase.GameResult gameResult)
    {
        IsPlaying = false;
        print("Game ended. Result : \n" + gameResult.ToString());
        OnGameEnded?.Invoke(gameResult);
    }
}
