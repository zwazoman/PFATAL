using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    //todo : scriptable object avec game settings ?
    public const float DEATH_MATCH_GAME_DURATION = 20;

    public static GameMode gameMode = GameMode.DeathMatch;

    private int _playersInScene = 0;
    
    public static GameManager Instance { get; private set ; }

    void Awake()
    {
        Instance = this;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SignalJoinRPC();
    }

    [Rpc(SendTo.Server)]
    void SignalJoinRPC()
    {
        print("Player loaded the map");
        _playersInScene++;
        if (_playersInScene == NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            print("Everyone laoded the map. start game !");
            StartGame(NetworkManager.Singleton.ConnectedClientsIds.ToList(),gameMode);
        }
    }
    
    //data
    public enum GameMode
    {
        DeathMatch,
        None
    }
    
    private GameRulesBase _serverGameRules;
    
    //synced events
   
    public event Action EventOnGameStarted;
    public event Action<GameRulesBase.GameResult> EventOnGameEnded;
    
    //synced variables
    public bool IsPlaying { get; private set ; } = false;
    public float TimeSinceGameStart => TimeStamp.Now - _gameStartTime;
    public LeaderBoardData LeaderBoard;
    public float _gameStartTime;
    
    private void StartGame(List<ulong> clientIDs,GameMode gameMode)
    {
        if (IsServer)
        {
            print("server start game");
            switch (gameMode)
            {
                case GameMode.DeathMatch:
                    _serverGameRules = new GameRulesDeathMatch(clientIDs,DEATH_MATCH_GAME_DURATION);
                    break;
                default:
                    throw new Exception("Game Mode not set");
                    break;
            };

            print("Link gamerules events");
            _serverGameRules.OnGameStarted += OnServerStartGameRPC;
            _serverGameRules.OnGameEnded += OnGameEnded;
            _serverGameRules.OnScoreBoardUpdated += OnScoreBoardUpdatedRpc;
            
            _serverGameRules.TriggerGameStart();
            
            //todo : set player status to inGame
        }
    }

    void OnGameEnded(GameRulesBase.GameResult gameResult)
    {
        OnServerEndGameRPC(gameResult);
        LeaderBoard.Clear();
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
        _gameStartTime = startTime;
        Debug.Log("Trigger OnGameStarted. start time : "+startTime);
        EventOnGameStarted?.Invoke();
    }
    
    [Rpc(SendTo.Everyone)]
    void OnServerEndGameRPC(GameRulesBase.GameResult gameResult)
    {
        IsPlaying = false;
        print("Game ended. Result : \n" + gameResult.ToString());
        EventOnGameEnded?.Invoke(gameResult);
    }
    

}
