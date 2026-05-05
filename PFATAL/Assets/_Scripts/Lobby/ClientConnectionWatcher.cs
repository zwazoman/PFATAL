using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Détecte si le host est mort en vérifiant le dernier heartbeat reçu.
/// </summary>
public class ClientConnectionWatcher : MonoBehaviour
{
    public static ClientConnectionWatcher Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float timeoutDuration = 6f;   // Secondes sans signal avant de considérer le host mort
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float redirectDelay = 2f;

    [Header("UI (optionnel)")]
    [SerializeField] private GameObject disconnectUI;

    private float _lastHeartbeatTime;
    private bool _isRedirecting = false;
    

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient 
            || NetworkManager.Singleton.IsHost)
        {
            enabled = false;
            return;
        }

        _lastHeartbeatTime = Time.time;
        StartCoroutine(WatchdogRoutine());
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
    
    public void OnHeartbeatReceived()
    {
        _lastHeartbeatTime = Time.time;
    }
    
    private IEnumerator WatchdogRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (_isRedirecting) yield break;

            float timeSinceLastBeat = Time.time - _lastHeartbeatTime;

            if (timeSinceLastBeat > timeoutDuration)
            {
                Debug.Log($"[Watchdog] Pas de heartbeat depuis {timeSinceLastBeat}s → Host mort.");
                StartCoroutine(ReturnToMainMenu());
            }
        }
    }
    
    private IEnumerator ReturnToMainMenu()
    {
        if (_isRedirecting) yield break;
        _isRedirecting = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (disconnectUI != null)
            disconnectUI.SetActive(true);

        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.Shutdown();

        yield return new WaitForSeconds(redirectDelay);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}