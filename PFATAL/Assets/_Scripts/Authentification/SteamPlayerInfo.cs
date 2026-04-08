using UnityEngine;
using Steamworks;

public class SteamPlayerInfo : MonoBehaviour
{
    public static SteamPlayerInfo Instance;

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
        yield return new WaitUntil(() => SteamPlayerList.Instance != null);
        SteamPlayerList.Instance.AddPlayerServerRpc(SteamId, PlayerName);
    }
}