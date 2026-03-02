using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public enum GameMode
    {
        DeathMatch,
        None
    }
    
    /// <summary>
    /// permet de set le game mode en dehors de la scène principale
    /// </summary>
    public static GameMode gameMode = GameMode.DeathMatch;

    private GameRulesBase serverGameRules;
    
    //synced events
    public event Action OnGameStarted;
    public event Action<GameRulesBase.GameResult> OnGameEnded;
    
    //synced variables
    public SortedSet<GameRulesBase.ScoreEntry> scoreBoard = new();
    public bool IsPlaying = false;
    
    private float _startTime;
    public float TimeSinceGameStart => TimeStamp.Now - _startTime;

    //todo : relier au lobby et au game rule
    
    public void StartGame(List<ulong> clientIDs,GameMode gameMode)
    {
        if (IsServer)
        {
            switch (gameMode)
            {
                case GameMode.DeathMatch:
                    serverGameRules = new GameRulesDeathMatch(clientIDs,100);
                    break;
                default:
                    throw new Exception("Game Mode not set");
                    break;
            };

            //serverGameRules.OnGameStarted += OnServerStartGameRPC;
            //serverGameRules.OnGameEnded += OnServerEndGameRPC;
            //serverGameRules.OnScoreBoardUpdated += OnScoreBoardUpdatedRpc;
        }
    }

    
//sync RPCs

    // [Rpc(SendTo.Everyone)]
    // void OnScoreBoardUpdatedRpc(SortedSet<GameRulesBase.ScoreEntry> newScoreboard)
    // {
    //     scoreBoard = newScoreboard;
    // }
    //
    // [Rpc(SendTo.Everyone)]
    // void OnServerStartGameRPC(float startTime)
    // {
    //     IsPlaying = true;
    //     _startTime = startTime;
    //     OnGameStarted?.Invoke();
    // }
    //
    // [Rpc(SendTo.Everyone)]
    // void OnServerEndGameRPC(GameRulesBase.GameResult gameResult)
    // {
    //     IsPlaying = false;
    //     print("Game ended. Result : \n" + gameResult.ToString());
    //     OnGameEnded?.Invoke(gameResult);
    // }
}
