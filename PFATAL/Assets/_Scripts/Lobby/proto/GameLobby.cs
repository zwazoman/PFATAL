using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// lobby :
/// liste des joueurs
/// gestion du statut des joueurs (pret/pas pret/en jeu)
/// lancement de la partie quand tout le monde est pret avec un gamemode et une map
/// </summary>
public class GameLobby : NetworkBehaviour
{
    
    //todo : scene selection, gamemode selection
    
    public static GameLobby Instance { get; private set; }
    
    public event Action<PlayerList> EventOnLobbyUpdated;
    public event Action<PlayerStatus> EventOnPlayerStatusChanged;
    
    private PlayerList _allPlayersInLobby = new();
    private int _lobbyIDCounter = 0;
    [SerializeField] private string _gameSceneName;
    public LobbyPlayerData LocalLobbyPlayerData {set; private get;}

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Init();
    }
    
    private void Init()
    {
        print("NetworkSpawn");
        LocalLobbyPlayerData = new LobbyPlayerData("_", 0, PlayerStatus.Waiting);
        if (NetworkManager.Singleton.IsServer)
        {
            // le server envoie la liste des joueurs aux autres clients quand un nouveau client rejoint la partie.
            print("=link events=");
            NetworkManager.Singleton.OnClientConnectedCallback += (ulong id) =>
            {
                print("OnClientConnected");
                int assignedLobbyID = _lobbyIDCounter++;
                _allPlayersInLobby.dictionnary[id] = new LobbyPlayerData("_", assignedLobbyID, PlayerStatus.Waiting);
                SyncPlayerListRPC(_allPlayersInLobby);
            };
            
            NetworkManager.Singleton.OnClientDisconnectCallback += (ulong id) =>
            {
                print("OnClientDisconnected");
                _allPlayersInLobby.dictionnary.Remove(id);
                SyncPlayerListRPC(_allPlayersInLobby);
            };

            int hostLobbyID = _lobbyIDCounter++;
            _allPlayersInLobby.dictionnary[NetworkManager.Singleton.LocalClientId] = new LobbyPlayerData("_", hostLobbyID, PlayerStatus.Waiting);
            EventOnLobbyUpdated?.Invoke(_allPlayersInLobby);

            //SyncPlayerListRPC(_allPlayersInLobby);
        }
    }

    /// <summary>
    /// Appelé par SteamPlayerList quand un joueur s'est enregistré avec ses infos Steam.
    /// </summary>
    public void OnSteamPlayerRegistered(ulong clientId, string steamName, string steamId)
    {
        if (!IsServer) return;
        if (!_allPlayersInLobby.dictionnary.TryGetValue(clientId, out LobbyPlayerData existing)) return;

        LobbyPlayerData updated = new(existing);
        updated.name = steamName;
        updated.steamId = steamId;
        _allPlayersInLobby.dictionnary[clientId] = updated;
        SyncPlayerListRPC(_allPlayersInLobby);
    }
    
    public void ToggleLocalPlayerStatus()
    {
        print("ToggleLocalPlayerStatus");
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
        print("SetPlayerStatus");
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
        print("SetLocalPlayerStatus");
        LobbyPlayerData lobbyPlayerData = new(LocalLobbyPlayerData);
        lobbyPlayerData.status = status;
        
        if (_allPlayersInLobby.dictionnary.TryGetValue(NetworkManager.Singleton.LocalClientId, out LobbyPlayerData existing))
            if (!string.IsNullOrEmpty(existing.name) && existing.name != "_")
                lobbyPlayerData.name = existing.name;

        LocalLobbyPlayerData = lobbyPlayerData;
        _allPlayersInLobby.dictionnary[NetworkManager.Singleton.LocalClientId] = lobbyPlayerData;

        print("lobby : "+_allPlayersInLobby.ToString());
        SyncPlayerListRPC(_allPlayersInLobby);
        EventOnPlayerStatusChanged?.Invoke(status);
    }
    
    [Rpc(SendTo.Everyone)]
    void SyncPlayerListRPC(PlayerList newPlayerList)
    {
        print("SyncPlayerListRPC");
        _allPlayersInLobby = newPlayerList;
        EventOnLobbyUpdated?.Invoke(_allPlayersInLobby);
        
        if (CheckForGameStart())
            StartGame();
    }

    bool CheckForGameStart()
    {
        if(!Debug.isDebugBuild)
            if (_allPlayersInLobby.dictionnary.Count < 2) return false;
        
        foreach (LobbyPlayerData player in _allPlayersInLobby.dictionnary.Values)
            if (player.status != PlayerStatus.Ready)
                return false;
        
        return true;
    }
    
    void StartGame()
    {
        //GameManager.gamemode = ...
        //GameManager.map = ...
        NetworkManager.Singleton.SceneManager.LoadScene(_gameSceneName, LoadSceneMode.Single);
    }
}

public enum PlayerStatus
{
    Waiting,
    Ready,
    InGame
}
public struct LobbyPlayerData : INetworkSerializable
{
    public LobbyPlayerData(string name, int lobbyID, PlayerStatus status)
    {
        this.name = name;
        if(this.name == null) Debug.LogError("ahhhhhh");
        this.steamId = "";
        this.lobbyID = lobbyID;
        this.status = status;
    }
    
    public LobbyPlayerData(LobbyPlayerData data)
    {
        this.name = data.name;
        if(this.name == null) Debug.LogError("ohhhhhh");
        this.steamId = data.steamId;
        this.lobbyID = data.lobbyID;
        this.status = data.status;
    }

    public string name;
    public string steamId;
    public int lobbyID; 
    public PlayerStatus status;
        
    public string DisplayName => string.IsNullOrEmpty(name) || name == "_" ? "player_" + lobbyID : name;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        Debug.Log("name is null : "+(name == null));
        Debug.Log("lobbyID is null : "+(lobbyID == null));
        Debug.Log("status is null : "+(status == null));
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref steamId);
        serializer.SerializeValue(ref lobbyID);
        serializer.SerializeValue(ref status);
    }

    public override string ToString()
    {
        return DisplayName + " (Steam: "+steamId+") - lobbyID : "+lobbyID+", "+status.ToString();
    }
}
public class PlayerList : INetworkSerializable
{
    /// <summary>
    /// netcode client id : playerdata
    /// </summary>
    public Dictionary<ulong,LobbyPlayerData> dictionnary = new();
    private ulong[] keys;
    private LobbyPlayerData[] values;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            keys = dictionnary.Keys.ToArray();
            values = dictionnary.Values.ToArray();
            Debug.Log("dico null : " + (dictionnary==null));
            Debug.Log("keys null : " + (keys==null));
            Debug.Log("values null : " + (values==null));
            serializer.SerializeValue(ref keys);
            serializer.SerializeValue(ref values);
        }
        else
        {
            serializer.SerializeValue(ref keys);
            serializer.SerializeValue(ref values);
            
            dictionnary.Clear();
            for (int i =0; i < keys.Length; i++)
            {
                dictionnary.Add(keys[i], values[i]);
            }
        }
    }

    public override string ToString()
    {
        string s = "";
        foreach (KeyValuePair<ulong, LobbyPlayerData> keyValuePair in dictionnary)
        {
            s+= keyValuePair.Key.ToString()+" : "+keyValuePair.Value.ToString()+'\n';
        }
        return s;
    }
}