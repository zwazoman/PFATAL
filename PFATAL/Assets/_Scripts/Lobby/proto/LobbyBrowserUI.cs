using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'affichage de la liste des lobbies disponibles et les interactions UI associées.
/// </summary>
public class LobbyBrowserUI : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private Transform lobbyListContent;
    [SerializeField] private GameObject lobbyCardPrefab;
    [SerializeField] private Button refreshButton;
    [SerializeField] private TextMeshProUGUI statusText;
    
    [Header("Panel")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject joinPanel;

    [Header("Dépendances")]
    [SerializeField] private LobbyBrowser lobbyBrowser;

    private void Awake()
    {
        if (lobbyBrowser == null)
            lobbyBrowser = FindFirstObjectByType<LobbyBrowser>();

        lobbyBrowser.OnLobbiesRefreshed += UpdateLobbyList;
        refreshButton.onClick.AddListener(OnRefreshClicked);
    }

    private void OnDestroy()
    {
        lobbyBrowser.OnLobbiesRefreshed -= UpdateLobbyList;
    }

    private async void OnRefreshClicked()
    {
        SetLoading(true);
        await lobbyBrowser.RefreshLobbies();
        SetLoading(false);
    }

    private void UpdateLobbyList(List<Lobby> lobbies)
    {
        foreach (Transform child in lobbyListContent)
            Destroy(child.gameObject);

        if (lobbies == null || lobbies.Count == 0)
        {
            statusText.text = "Aucun lobby disponible.";
            return;
        }

        statusText.text = $"{lobbies.Count} lobby(s) disponible(s)";

        foreach (Lobby lobby in lobbies)
        {
            GameObject card = Instantiate(lobbyCardPrefab, lobbyListContent);
            LobbyCard cardScript = card.GetComponent<LobbyCard>();

            if (cardScript != null)
                cardScript.Setup(lobby, OnJoinLobby);
        }
    }

    private async void OnJoinLobby(Lobby lobby)
    {
        SetLoading(true);
        refreshButton.interactable = false;       

        bool success = await NetworkConnectionManager.Instance.StartClientById(lobby.Id);

        if (!success)
        {
            SetLoading(false, "Impossible de rejoindre ce lobby.");
            refreshButton.interactable = true;
        }
    }

    private void SetLoading(bool isLoading, string message = "Chargement...")
    {
        loadingPanel.SetActive(isLoading);
        loadingText.text = isLoading ? message : "";
        statusText.text = isLoading ? message : "";
        refreshButton.interactable = !isLoading;
    }
}
