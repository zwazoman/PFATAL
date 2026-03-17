using Unity.Netcode;
using UnityEngine;
using TMPro;

public class GameSceneUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject codePanel;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TextMeshProUGUI playerCountText;

    private void Start()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            DisplayLobbyCode();
        }
        else
        {
            if (codePanel != null)
            {
                codePanel.SetActive(false);
            }
        }

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        UpdatePlayerCount();
    }

    private void DisplayLobbyCode()
    {
        if (LobbyManager.Instance != null)
        {
            var lobby = LobbyManager.Instance.GetCurrentLobby();

            if (lobby != null && lobbyCodeText != null)
            {
                lobbyCodeText.text = $"Code: {lobby.LobbyCode}";

                if (codePanel != null)
                {
                    codePanel.SetActive(true);
                }

                Debug.Log($"[GameSceneUI] Code du lobby affiché: {lobby.LobbyCode}");
            }
            else
            {
                Debug.LogWarning("[GameSceneUI] Impossible de récupérer le lobby");
            }
        }
    }
    private void UpdatePlayerCount()
    {
        if (NetworkManager.Singleton != null && playerCountText != null)
        {
            int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
            playerCountText.text = $"Joueurs: {playerCount}/8";
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        UpdatePlayerCount();
        Debug.Log($"[GameSceneUI] Joueur connecté: {clientId}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        UpdatePlayerCount();
        Debug.Log($"[GameSceneUI] Joueur déconnecté: {clientId}");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}