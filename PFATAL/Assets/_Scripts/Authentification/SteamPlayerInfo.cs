using Discord;
using UnityEngine;
using Steamworks;
using Unity.Netcode;
using NetworkManager = Unity.Netcode.NetworkManager;

public class SteamPlayerInfo : MonoBehaviour
{
    const ulong NULL_NETWORK_ID = ulong.MaxValue;
    
    public static SteamPlayerInfo Instance;

    public ulong NetworkClientId { get; private set; } = NULL_NETWORK_ID;
    public string SteamId { get; private set; }
    public string PlayerName { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    void Start()
    {
        InitSteamData();
        StartCoroutine(WaitAndSend());
        
        NetworkManager.Singleton.OnClientStarted += () =>
        {
            NetworkClientId = NetworkManager.Singleton.LocalClientId;
        };
    }

    void InitSteamData()
    {

        if (!SteamManager.Initialized) { Debug.LogError("[SteamPlayerInfo] Steam non initialisé !"); return; }
        SteamId = SteamUser.GetSteamID().ToString();
        PlayerName = SteamFriends.GetPersonaName();
        Debug.Log($"[SteamPlayerInfo] {PlayerName} ({SteamId})");
    }

    System.Collections.IEnumerator WaitAndSend()
    {
        //attend d'être connecté à netcode et à steam
        yield return new WaitUntil(() =>
            SteamPlayerList.Instance != null 
            && NetworkClientId != NULL_NETWORK_ID);
        
        //Met à jour la liste de tous les clients
        SteamPlayerList.Instance.AddPlayerRpc(NetworkClientId,SteamId, PlayerName);
    }
}