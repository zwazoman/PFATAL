using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUI : MonoBehaviour
{
    [Header("Panneaux")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private GameObject loadingPanel;

    [Header("Boutons Menu Principal")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    [Header("Host Panel")]
    [SerializeField] private TMP_InputField lobbyNameInput;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button cancelHostButton;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    [Header("Join Panel")]
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private Button startJoinButton;
    [SerializeField] private Button cancelJoinButton;

    [Header("Loading Panel")]
    [SerializeField] private TextMeshProUGUI loadingText;

    private void Start()
    {
        hostButton.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
        startHostButton.onClick.AddListener(OnStartHostClicked);
        cancelHostButton.onClick.AddListener(OnCancelHostClicked);
        startJoinButton.onClick.AddListener(OnStartJoinClicked);
        cancelJoinButton.onClick.AddListener(OnCancelJoinClicked);

        ShowMainPanel();
    }

    private void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    private void ShowHostPanel()
    {
        mainPanel.SetActive(false);
        hostPanel.SetActive(true);
        joinPanel.SetActive(false);
        loadingPanel.SetActive(false);

        lobbyCodeText.text = "";
    }

    private void ShowJoinPanel()
    {
        mainPanel.SetActive(false);
        hostPanel.SetActive(false);
        joinPanel.SetActive(true);
        loadingPanel.SetActive(false);
    }

    private void ShowLoadingPanel(string message)
    {
        loadingPanel.SetActive(true);
        loadingText.text = message;
        mainPanel.SetActive(false);
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);
    }

    private void OnHostButtonClicked()
    {
        ShowHostPanel();
    }

    private void OnJoinButtonClicked()
    {
        ShowJoinPanel();
    }

    private async void OnStartHostClicked()
    {
        string lobbyName = string.IsNullOrEmpty(lobbyNameInput.text) ? "MyGame" : lobbyNameInput.text;

        ShowLoadingPanel("Cr�ation de la partie...");

        bool success = await NetworkConnectionManager.Instance.StartHost(lobbyName);

        if (!success)
        {
            Debug.LogError("[Menu] �chec du d�marrage de l'host");
            ShowHostPanel();
            lobbyCodeText.text = "Erreur lors de la cr�ation";
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

            Debug.Log("[Menu] Host d�marr� avec succ�s");
        }
    }

    private void OnCancelHostClicked()
    {
        ShowMainPanel();
    }

    private async void OnStartJoinClicked()
    {
        string joinCode = joinCodeInput.text.Trim();

        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogWarning("[Menu] Code de jointure vide");
            return;
        }

        ShowLoadingPanel("Connexion � la partie...");

        bool success = await NetworkConnectionManager.Instance.StartClient(joinCode);

        if (!success)
        {
            Debug.LogError("[Menu] �chec de la connexion");
            ShowJoinPanel();
        }
        else
        {
            Debug.Log("[Menu] Client connect� avec succ�s");
        }
    }

    private void OnCancelJoinClicked()
    {
        ShowMainPanel();
    }
}