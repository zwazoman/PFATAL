using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkConnectionManager : MonoBehaviour
{
    public static NetworkConnectionManager Instance { get; private set; }

    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Prefabs")]
    [SerializeField] private GameObject networkManagerPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> StartHost(string lobbyName = "MyGame")
    {
        // ✅ Désactive le NetworkObject de l'AudioManager pour éviter le conflit de hash
        var audioNetObj = FindObjectOfType<AudioManager>()?.GetComponent<NetworkObject>();
        if (audioNetObj != null) audioNetObj.enabled = false;

        // ✅ Recrée le NetworkManager à chaque session
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
            await Task.Yield();
        }
        var nmGO = Instantiate(networkManagerPrefab);
        DontDestroyOnLoad(nmGO);
        await Task.Delay(500);

        bool servicesInitialized = await UnityServicesManager.Instance.InitializeUnityServices();
        if (!servicesInitialized)
        {
            Debug.LogError("[Network] Impossible d'initialiser Unity Services");
            if (audioNetObj != null) audioNetObj.enabled = true;
            return false;
        }

        string lobbyCode = await LobbyManager.Instance.CreateLobby(lobbyName);
        if (string.IsNullOrEmpty(lobbyCode))
        {
            Debug.LogError("[Network] Impossible de créer le lobby");
            if (audioNetObj != null) audioNetObj.enabled = true;
            return false;
        }

        string relayJoinCode = await RelayManager.Instance.CreateRelayAllocation();
        if (string.IsNullOrEmpty(relayJoinCode))
        {
            Debug.LogError("[Network] Impossible de créer l'allocation Relay");
            await LobbyManager.Instance.DeleteLobby();
            if (audioNetObj != null) audioNetObj.enabled = true;
            return false;
        }

        bool lobbyUpdated = await LobbyManager.Instance.UpdateLobbyRelayCode(relayJoinCode);
        if (!lobbyUpdated)
        {
            Debug.LogError("[Network] Impossible de mettre à jour le lobby avec le code Relay");
            await LobbyManager.Instance.DeleteLobby();
            if (audioNetObj != null) audioNetObj.enabled = true;
            return false;
        }

        bool hostStarted = NetworkManager.Singleton.StartHost();
        if (!hostStarted)
        {
            Debug.LogError("[Network] Impossible de démarrer Netcode en mode host");
            await LobbyManager.Instance.DeleteLobby();
            if (audioNetObj != null) audioNetObj.enabled = true;
            return false;
        }

        if (audioNetObj != null) audioNetObj.enabled = true;

        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        return true;
    }

    public async Task<bool> StartClient(string lobbyCode)
    {
        bool servicesInitialized = await UnityServicesManager.Instance.InitializeUnityServices();
        if (!servicesInitialized)
        {
            Debug.LogError("[Network] Impossible d'initialiser Unity Services");
            return false;
        }

        bool lobbyJoined = await LobbyManager.Instance.JoinLobbyByCode(lobbyCode);
        if (!lobbyJoined)
        {
            Debug.LogError("[Network] Impossible de rejoindre le lobby");
            return false;
        }

        string relayJoinCode = null;
        int attempts = 0;
        while (string.IsNullOrEmpty(relayJoinCode) && attempts < 10)
        {
            await Task.Delay(500);
            relayJoinCode = await LobbyManager.Instance.GetRelayJoinCode();
            attempts++;
        }

        if (string.IsNullOrEmpty(relayJoinCode))
        {
            Debug.LogError("[Network] Impossible de récupérer le code Relay du lobby");
            await LobbyManager.Instance.LeaveLobby();
            return false;
        }

        bool relayJoined = await RelayManager.Instance.JoinRelayAllocation(relayJoinCode);
        if (!relayJoined)
        {
            Debug.LogError("[Network] Impossible de rejoindre l'allocation Relay");
            await LobbyManager.Instance.LeaveLobby();
            return false;
        }

        bool clientStarted = NetworkManager.Singleton.StartClient();
        if (!clientStarted)
        {
            Debug.LogError("[Network] Impossible de démarrer Netcode en mode client");
            await LobbyManager.Instance.LeaveLobby();
            return false;
        }

        return true;
    }

    public async Task<bool> StartClientById(string lobbyId)
    {
        while (!UnityServicesManager.Instance.IsInitialized)
            await Task.Delay(100);

        bool lobbyJoined = await LobbyManager.Instance.JoinLobbyById(lobbyId);
        if (!lobbyJoined) { Debug.LogError("[Network] Impossible de rejoindre le lobby"); return false; }

        string relayJoinCode = null;
        int attempts = 0;
        while (string.IsNullOrEmpty(relayJoinCode) && attempts < 10)
        {
            await Task.Delay(500);
            relayJoinCode = await LobbyManager.Instance.GetRelayJoinCode();
            attempts++;
        }

        if (string.IsNullOrEmpty(relayJoinCode)) { await LobbyManager.Instance.LeaveLobby(); return false; }

        bool relayJoined = await RelayManager.Instance.JoinRelayAllocation(relayJoinCode);
        if (!relayJoined) { await LobbyManager.Instance.LeaveLobby(); return false; }

        bool clientStarted = NetworkManager.Singleton.StartClient();
        if (!clientStarted) { await LobbyManager.Instance.LeaveLobby(); return false; }

        return true;
    }

    public async Task SetLobbyLocked(bool locked)
    {
        try
        {
            await LobbyManager.Instance.SetLobbyLocked(locked);
            Debug.Log($"[Network] Lobby {(locked ? "verrouillé" : "déverrouillé")}.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Network] Erreur changement de verrou lobby : {e.Message}");
        }
    }

    public async Task Disconnect()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();

            while (NetworkManager.Singleton.IsListening)
                await Task.Yield();
        }

        if (LobbyManager.Instance.IsHost())
            await LobbyManager.Instance.DeleteLobby();
        else
            await LobbyManager.Instance.LeaveLobby();

        // ✅ Détruit le NetworkManager — recréé au prochain StartHost
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);

        Debug.Log("[Network] Déconnexion terminée");
    }

    private void OnApplicationQuit()
    {
        _ = Disconnect();
    }
}