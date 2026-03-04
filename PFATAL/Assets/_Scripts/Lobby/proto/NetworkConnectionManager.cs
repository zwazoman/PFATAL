using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkConnectionManager : MonoBehaviour
{
    public static NetworkConnectionManager Instance { get; private set; }

    [Header("Sc�nes")]
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
            Debug.LogError("[Network] Impossible de cr�er le lobby");
            return false;
        }

        //Debug.Log($"[Network] Code du lobby: {lobbyCode}");

        string relayJoinCode = await RelayManager.Instance.CreateRelayAllocation();
        if (string.IsNullOrEmpty(relayJoinCode))
        {
            Debug.LogError("[Network] Impossible de cr�er l'allocation Relay");
            await LobbyManager.Instance.DeleteLobby();
            return false;
        }

        bool lobbyUpdated = await LobbyManager.Instance.UpdateLobbyRelayCode(relayJoinCode);
        if (!lobbyUpdated)
        {
            Debug.LogError("[Network] Impossible de mettre � jour le lobby avec le code Relay");
            await LobbyManager.Instance.DeleteLobby();
            return false;
        }

        bool hostStarted = NetworkManager.Singleton.StartHost();
        if (!hostStarted)
        {
            Debug.LogError("[Network] Impossible de d�marrer Netcode en mode host");
            await LobbyManager.Instance.DeleteLobby();
            return false;
        }

        //Debug.Log("[Network] Host d�marr� avec succ�s");

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
            Debug.LogError("[Network] Impossible de r�cup�rer le code Relay du lobby");
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
            Debug.LogError("[Network] Impossible de d�marrer Netcode en mode client");
            await LobbyManager.Instance.LeaveLobby();
            return false;
        }

        //Debug.Log("[Network] Client connect� avec succ�s");
        return true;
    }

    public async Task Disconnect()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("[Network] Netcode arr�t�");
        }

        if (LobbyManager.Instance.IsHost())
        {
            await LobbyManager.Instance.DeleteLobby();
        }
        else
        {
            await LobbyManager.Instance.LeaveLobby();
        }

        Debug.Log("[Network] D�connexion termin�e");
    }

    private void OnApplicationQuit()
    {
        _ = Disconnect();
    }
}