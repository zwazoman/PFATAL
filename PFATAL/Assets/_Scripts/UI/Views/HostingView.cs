using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostingView : View
{
    [Header("Global References")]
    [SerializeField] LoadingView _loadingView;

    [Header("Host References")]
    [SerializeField] private TMP_InputField lobbyNameInput;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button cancelHostButton;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    public async void StartHost()
    {
        string lobbyName = string.IsNullOrEmpty(lobbyNameInput.text) ? "MyGame" : lobbyNameInput.text;

        _loadingView.loadingText.text = "Creation of the game...";
        ViewManager.Instance.SwapView(_loadingView);

        bool success = await NetworkConnectionManager.Instance.StartHost(lobbyName);

        if (!success)
        {
            ViewManager.Instance.SwapView(this);
            lobbyCodeText.text = "Error during creation";
            lobbyCodeText.color = Color.red;
        }
        else
        {
            var lobby = LobbyManager.Instance.GetCurrentLobby();
            if (lobby != null)
            {
                lobbyCodeText.text = $"Game Code: {lobby.LobbyCode}";
                lobbyCodeText.color = Color.green;
            }
        }
    }
}
