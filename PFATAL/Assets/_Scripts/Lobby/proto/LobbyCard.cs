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

    public void Setup(Lobby lobby, LobbyBrowserUI ui)
    {
        this._lobby = lobby;

        lobbyNameText.text  = lobby.Name;
        playerCountText.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        hostText.text        = $"Code: {lobby.LobbyCode}";

        bool isFull = lobby.Players.Count >= lobby.MaxPlayers;
        joinButton.interactable = !isFull;
        
        print("lobby is full : "+isFull);
        joinButton.targetGraphic.color = isFull ?
            new Color(50/255f,55/255f,74/255f)
            : new Color(0x8C/255f,0xA1/255f,0x4B/255f);
        Debug.Log(joinButton.name , joinButton);
        joinButton.onClick.AddListener(() => ui.JoinLobby(lobby));
        joinButton.onClick.AddListener(()=>print("click"));
        //onJoinCallback += (_) => print("evet !!!!");
        Debug.Log(joinButton.onClick);
    }
}
