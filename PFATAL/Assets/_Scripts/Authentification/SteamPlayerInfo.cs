using UnityEngine;
using Steamworks;
using NetworkManager = Unity.Netcode.NetworkManager;

public class SteamPlayerInfo : MonoBehaviour
{
    const ulong NULL_NETWORK_ID = ulong.MaxValue;
    
    public static SteamPlayerInfo Instance;
    public ulong NetworkClientId { get; private set; } = NULL_NETWORK_ID;
    public string SteamId { get; private set; }
    public string PlayerName { get; private set; }
    
    public string tempUnityId;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    void Start()
    {
        InitSteamData();
        WaitAndSend_Async();
        UnityServicesManager.Instance.GetPlayerId();
        tempUnityId = UnityServicesManager.Instance.GetPlayerId();
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
    
    async void WaitAndSend_Async()
    {
        Debug.Log("[SteamPlayerInfo] WaitAndSend_Async");
        while (SteamPlayerList.Instance == null 
               || NetworkClientId == NULL_NETWORK_ID)
        {
            print($"[SteamPlayerInfo] {SteamPlayerList.Instance} ({NetworkClientId})");
            await Awaitable.NextFrameAsync();
        }
        
        SteamPlayerList.Instance.AddPlayerRpc(NetworkClientId,SteamId, PlayerName, tempUnityId);
    }
}