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

    private Lobby _lobby;
    private Action<Lobby> onJoinCallback;

    public void Setup(Lobby lobby, Action<Lobby> onJoinCallback)
    {
        this._lobby = lobby;
        this.onJoinCallback = onJoinCallback;

        lobbyNameText.text  = lobby.Name;
        playerCountText.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        hostText.text        = $"Code: {lobby.LobbyCode}";

        bool isFull = lobby.Players.Count >= lobby.MaxPlayers;
        joinButton.interactable = !isFull;
        
        print("lobby is full : "+isFull);
        joinButton.targetGraphic.color = isFull ?
            new Color(50/255f,55/255f,74/255f)
            : new Color(0x8C/255f,0xA1/255f,0x4B/255f);
        
        joinButton.onClick.AddListener(() => onJoinCallback?.Invoke(lobby));
    }
}
