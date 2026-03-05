using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// lobby :
/// liste des joueurs
/// gestion du statut des joueurs (pret/pas pret/en jeu)
/// lancement de la partie quand tout le monde est pret avec un gamemode et une map
/// </summary>
public class GameLobby : NetworkBehaviour
{
    [Header("SceneReferences")]
    [SerializeField] GameManager _gameManager;
    
    public event Action<PlayerList> EventOnLobbyUpdated;
    public event Action<PlayerStatus> EventOnPlayerStatusChanged;
    
    private PlayerList _allPlayersInLobby = new();
    public LobbyPlayerData LocalLobbyPlayerData {set; private get;}
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            // le server envoie la liste des joueurs aux autres clients quand un nouveau client rejoint la partie.
            
            NetworkManager.Singleton.OnClientConnectedCallback += (ulong id) =>
            {
                //todo : steam name, lobbyID
                _allPlayersInLobby.dictionnary[id] = new LobbyPlayerData("",0,PlayerStatus.Waiting);
                SyncPlayerListRPC(_allPlayersInLobby);
            };
            
            NetworkManager.Singleton.OnClientDisconnectCallback += (ulong id) =>
            {
                _allPlayersInLobby.dictionnary.Remove(id);
                SyncPlayerListRPC(_allPlayersInLobby);
            };
            
        }
    }
    
    public void ToggleLocalPlayerStatus()
    {
        SetLocalPlayerStatus(LocalLobbyPlayerData.status switch
        {
            PlayerStatus.Waiting => PlayerStatus.Ready,
            PlayerStatus.Ready => PlayerStatus.Waiting,
            PlayerStatus.InGame => PlayerStatus.InGame,
            _ => throw new ArgumentOutOfRangeException()
        });
    }


    public void SetPlayerStatus(ulong clientID, PlayerStatus status)
    {
        LobbyPlayerData lobbyPlayerData = new(_allPlayersInLobby.dictionnary[clientID]);
        lobbyPlayerData.status = status;
        _allPlayersInLobby.dictionnary[clientID] = lobbyPlayerData;
    }
    
    /// <summary>
    /// change le statut du joueur pret.
    /// </summary>
    /// <param name="status"></param>
    public void SetLocalPlayerStatus(PlayerStatus status)
    {
        LobbyPlayerData lobbyPlayerData = new(LocalLobbyPlayerData);
        lobbyPlayerData.status = status;
        LocalLobbyPlayerData = lobbyPlayerData;
        _allPlayersInLobby.dictionnary[NetworkManager.Singleton.LocalClientId] = lobbyPlayerData;
        
        SyncPlayerListRPC(_allPlayersInLobby);
        EventOnPlayerStatusChanged?.Invoke(status);
    }
    
    [Rpc(SendTo.Everyone)]
    void SyncPlayerListRPC(PlayerList newPlayerList)
    {
        _allPlayersInLobby = newPlayerList;
        EventOnLobbyUpdated?.Invoke(_allPlayersInLobby);
    }
}

public enum PlayerStatus
{
    Waiting,
    Ready,
    InGame
}
public struct LobbyPlayerData : INetworkSerializeByMemcpy
{
    public LobbyPlayerData(string name, int lobbyID, PlayerStatus status)
    {
        this.name = name;
        this.lobbyID = lobbyID;
        this.status = status;
    }
    public LobbyPlayerData(LobbyPlayerData data)
    {
        this.name = data.name;
        this.lobbyID = data.lobbyID;
        this.status = data.status;
    }

    public string name;
    public int lobbyID; 
    public PlayerStatus status;
        
    //todo : steam name
    string DisplayName => "player_"+lobbyID;
}
public class PlayerList : INetworkSerializable
{
    /// <summary>
    /// netcode client id : playerdata
    /// </summary>
    public Dictionary<ulong,LobbyPlayerData> dictionnary = new();

    //todo
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new NotImplementedException();
    }
}

