using Steamworks;
using UnityEngine;

public class DiscordUpdater : MonoBehaviour
{
    public string details;
    public string state;
    public int currentPartySize;
    public int maxPartySize;
    [Space]
    public string largeImageKey;
    public string largeText;
    public string smallImageKey;
    public string smallText;

    [SerializeField] private bool _isInLobbyScene;
    [SerializeField] private GameLobby _gameLobby;

    [SerializeField] private bool _isInGame;

    private float _timer;
    Discord_Controller controller;

    private void Start()
    {
        if (Discord_Controller.Instance == null)
        {
            //Debug.LogError("Discord_Controller instance not found. Please ensure a Discord_Controller is present in the scene.");
            return;
        }
        else
        {
            controller = Discord_Controller.Instance;

            if (_isInLobbyScene)
            {
                _gameLobby.EventOnLobbyUpdated += UpdateDiscord;

                controller.currentPartySize = _gameLobby._allPlayersInLobby.dictionnary.Count;
                controller.maxPartySize = 8;
            }
        }

        UpdateDiscord(new());
    }

    public void UpdateDiscord(PlayerList list)
    {
        if (controller == null)
        {
            //Debug.LogError("Discord_Controller instance not found. Please ensure a Discord_Controller is present in the scene.");
            return;
        }
        else
        {
            controller.details = details;

            controller.largeImageKey = largeImageKey;
            controller.largeText = largeText;
            controller.smallImageKey = smallImageKey;
            controller.smallText = smallText;


            if (_isInLobbyScene)
            {
               currentPartySize = _gameLobby._allPlayersInLobby.dictionnary.Count;
            }
            if (_isInGame)
            {
                currentPartySize = controller.currentPartySize;
            }

            controller.currentPartySize = currentPartySize;
            controller.maxPartySize = maxPartySize;
        }
    }
}