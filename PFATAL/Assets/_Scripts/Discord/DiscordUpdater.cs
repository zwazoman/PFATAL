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
            Debug.LogError("Discord_Controller instance not found. Please ensure a Discord_Controller is present in the scene.");
            return;
        }
        else
        {
            controller = Discord_Controller.Instance;
        }

        UpdateDiscord();
    }

    private void Update()
    {
        if (controller == null)
        {
            Debug.LogError("Discord_Controller instance not found. Please ensure a Discord_Controller is present in the scene.");
            return;
        }
        else if (_isInLobbyScene)
        {
            controller.currentPartySize = _gameLobby._allPlayersInLobby.dictionnary.Count;
            controller.maxPartySize = 8;
        }
        else if ( _isInGame)
        {
            
        }
    }

    public void UpdateDiscord()
    {
        if (controller == null)
        {
            Debug.LogError("Discord_Controller instance not found. Please ensure a Discord_Controller is present in the scene.");
            return;
        }
        else
        {
            controller.details = details;

            controller.largeImageKey = largeImageKey;
            controller.largeText = largeText;
            controller.smallImageKey = smallImageKey;
            controller.smallText = smallText;

            if (maxPartySize == 0 && currentPartySize == 0)
            {
                controller.currentPartySize = 0;
                controller.maxPartySize = 0;
            }
        }
    }
}
