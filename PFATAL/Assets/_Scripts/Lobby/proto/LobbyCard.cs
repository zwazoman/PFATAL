using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCard : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI hostText;
    [SerializeField] private Button joinButton;

    private Lobby lobby;
    private Action<Lobby> onJoinCallback;

    public void Setup(Lobby lobby, Action<Lobby> onJoinCallback)
    {
        this.lobby = lobby;
        this.onJoinCallback = onJoinCallback;

        lobbyNameText.text  = lobby.Name;
        playerCountText.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        hostText.text        = $"Code: {lobby.LobbyCode}";

        bool isFull = lobby.Players.Count >= lobby.MaxPlayers;
        joinButton.interactable = !isFull;
        joinButton.onClick.AddListener(() => onJoinCallback?.Invoke(lobby));
    }
}
