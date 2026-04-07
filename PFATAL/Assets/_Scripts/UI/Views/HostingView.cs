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

        _loadingView.loadingText.text = "Creation de la partie...";
        ViewManager.Instance.SwapView(_loadingView);

        bool success = await NetworkConnectionManager.Instance.StartHost(lobbyName);

        if (!success)
        {
            Debug.LogError("[Menu] echec du demarrage de l'host");
            ViewManager.Instance.SwapView(this);
            lobbyCodeText.text = "Erreur lors de la creation";
            lobbyCodeText.color = Color.red;
        }
        else
        {
            var lobby = LobbyManager.Instance.GetCurrentLobby();
            if (lobby != null)
            {
                lobbyCodeText.text = $"Code de la partie: {lobby.LobbyCode}";
                lobbyCodeText.color = Color.green;
            }

            Debug.Log("[Menu] Host demarre avec succes");
        }
    }
}
