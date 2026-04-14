using System;
using System.Collections.Generic;
using System.Linq;
using _scripts.PlayerCharacter;
using NetworkTime;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkTimeSyncManager _timeSyncManager;
    
    //todo : scriptable object avec game settings ?
    public const float DEATH_MATCH_GAME_DURATION = 10f;

    public static GameMode gameMode = GameMode.DeathMatch;
    
    private static Dictionary<ulong,PermanentPlayerIdentity> _playerIdentities = new();
     
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

    /// <summary>
    /// appelé par chaque client au début pour signaler au server qu'il a fini de charger la map.
    /// </summary>
    [Rpc(SendTo.Server)]
    void SignalJoinRPC()
    {
        print("Player loaded the map");
        _playersInScene++;
        
        //quand tous les clients sont connectés
        if (_playersInScene == NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            print("Everyone laoded the map.");
            InitializeGame();
        }
    }

    async void InitializeGame()
    {
        await _timeSyncManager.SyncClientTimestamps();
        SyncPlayerIdentitiesToClientsRPC(_playerIdentities.Keys.ToArray(), _playerIdentities.Values.ToArray());
        StartGame(NetworkManager.Singleton.ConnectedClientsIds.ToList(), gameMode);
    }

    [Rpc(SendTo.Everyone)]
    void SyncPlayerIdentitiesToClientsRPC(ulong[] ids, PermanentPlayerIdentity[] identities)
    {
        _playerIdentities.Clear();
        for (int i = 0; i < ids.Length; i++)
            _playerIdentities[ids[i]] = identities[i];
        Debug.Log($"[GameManager] PlayerIdentities synchronisées : {_playerIdentities.Count}");
    }

    /// <summary>
    /// Donne l'identité permanente d'un joueur (steam...) via NetworkClientId de Netcode
    /// </summary>
    public static PermanentPlayerIdentity GetPlayerIdentity(ulong playerClientID)
    {
        return _playerIdentities[playerClientID];
    }

    public static void SetPlayerIdentities(Dictionary<ulong,PermanentPlayerIdentity> playerIdentities)
    {
        _playerIdentities = playerIdentities;
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
    public float TimeSinceGameStart => TimeStamp.Now - _gameStartTime;
    public LeaderBoardData LeaderBoard;
    public float _gameStartTime;
    public bool IsPlaying { get; private set ; } = false;
    public bool IsGameOver { get; private set; } = false;

    public PlayerCharacter localPlayerCharacter { get; private set; }
    
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

    public PlayerCharacter GetPlayerCharacter(ulong playerClientID)
    {
        return _serverGameRules.GetPlayerCharacter(playerClientID);
    }

    public void SetLocalPlayer(PlayerCharacter playerchara, ulong ownerClientId)
    {
        SetLocalPlayerRpc(playerchara, RpcTarget.Single(ownerClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SetLocalPlayerRpc(NetworkBehaviourReference playerBhvRef, RpcParams rpcParams = default)
    {
        NetworkBehaviour playerNTWBhv = null;

        if(playerBhvRef.TryGet(out playerNTWBhv))
        {
            if(playerNTWBhv.TryGetComponent(out PlayerCharacter playercharacter))
                localPlayerCharacter = playercharacter;
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
        IsGameOver = false;
        IsPlaying = true;
        _gameStartTime = startTime;
        Debug.Log("Trigger OnGameStarted. start time : "+startTime);
        EventOnGameStarted?.Invoke();
    }
    
    [Rpc(SendTo.Everyone)]
    void OnServerEndGameRPC(GameRulesBase.GameResult gameResult)
    {
        IsPlaying = false;
        IsGameOver = true;
        print("Game ended. Shared result : \n" + gameResult.ToString());
        EventOnGameEnded?.Invoke(gameResult);
    }
    

}
