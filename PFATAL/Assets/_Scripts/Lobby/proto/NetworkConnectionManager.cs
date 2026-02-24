using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkConnectionManager : MonoBehaviour
{
    public static NetworkConnectionManager Instance { get; private set; }

    [Header("Scènes")]
    [SerializeField] private string gameSceneName = "GameScene";

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
        bool servicesInitialized = await UnityServicesManager.Instance.InitializeUnityServices();
        if (!servicesInitialized)
        {
            Debug.LogError("[Network] Impossible d'initialiser Unity Services");
            return false;
        }

        string lobbyCode = await LobbyManager.Instance.CreateLobby(lobbyName);
        if (string.IsNullOrEmpty(lobbyCode))
        {
            Debug.LogError("[Network] Impossible de créer le lobby");
            return false;
        }

        Debug.Log($"[Network] Code du lobby: {lobbyCode}");

        string relayJoinCode = await RelayManager.Instance.CreateRelayAllocation();
        if (string.IsNullOrEmpty(relayJoinCode))
        {
            Debug.LogError("[Network] Impossible de créer l'allocation Relay");
            await LobbyManager.Instance.DeleteLobby();
            return false;
        }

        bool lobbyUpdated = await LobbyManager.Instance.UpdateLobbyRelayCode(relayJoinCode);
        if (!lobbyUpdated)
        {
            Debug.LogError("[Network] Impossible de mettre à jour le lobby avec le code Relay");
            await LobbyManager.Instance.DeleteLobby();
            return false;
        }

        bool hostStarted = NetworkManager.Singleton.StartHost();
        if (!hostStarted)
        {
            Debug.LogError("[Network] Impossible de démarrer Netcode en mode host");
            await LobbyManager.Instance.DeleteLobby();
            return false;
        }

        Debug.Log("[Network] Host démarré avec succès");

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

        Debug.Log("[Network] Client connecté avec succès");
        return true;
    }

    public async Task Disconnect()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("[Network] Netcode arrêté");
        }

        if (LobbyManager.Instance.IsHost())
        {
            await LobbyManager.Instance.DeleteLobby();
        }
        else
        {
            await LobbyManager.Instance.LeaveLobby();
        }

        Debug.Log("[Network] Déconnexion terminée");
    }

    private void OnApplicationQuit()
    {
        _ = Disconnect();
    }
}