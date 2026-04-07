using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoiningView : View
{
    [Header("Global References")]
    [SerializeField] LoadingView _loadingView;

    [Header("Join References")]
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private Button startJoinButton;
    [SerializeField] private Button cancelJoinButton;

    public async void StartJoin()
    {
        string joinCode = joinCodeInput.text.Trim();

        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogWarning("[Menu] Code de jointure vide");
            return;
        }

        _loadingView.loadingText.text = "Connexion a la partie...";
        ViewManager.Instance.SwapView(_loadingView);

        bool success = await NetworkConnectionManager.Instance.StartClient(joinCode);

        if (!success)
        {
            Debug.LogError("[Menu] echec de la connexion");
            ViewManager.Instance.SwapView(this);
        }
        else
        {
            Debug.Log("[Menu] Client connecte avec succes");
        }
    }
}
