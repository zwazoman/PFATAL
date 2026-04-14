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
        tempUnityId = UnityServicesManager.Instance.GetPlayerId();

        // Si déjà connecté au moment du Start
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            NetworkClientId = NetworkManager.Singleton.LocalClientId;
            Debug.Log($"[SteamPlayerInfo] NetworkClientId défini directement : {NetworkClientId}");
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                // On vérifie que c'est bien nous
                if (id == NetworkManager.Singleton.LocalClientId)
                {
                    NetworkClientId = id;
                    Debug.Log($"[SteamPlayerInfo] NetworkClientId défini via callback : {NetworkClientId}");
                }
            };
        }

        WaitAndSend_Async();
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
               || tempUnityId == null
               || NetworkClientId == NULL_NETWORK_ID)
        {
            print($"[SteamPlayerInfo] waiting... SteamPlayerList={SteamPlayerList.Instance} | tempUnityId={tempUnityId} | NetworkClientId={NetworkClientId}");
            await Awaitable.NextFrameAsync();
        }
        
        Debug.Log($"[SteamPlayerInfo] Envoi : {PlayerName} | {SteamId} | {NetworkClientId} | {tempUnityId}");
        SteamPlayerList.Instance.AddPlayerRpc(NetworkClientId, SteamId, PlayerName, tempUnityId);
    }
}