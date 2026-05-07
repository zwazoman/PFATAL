using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // ou UnityEngine.UI si tu utilises Text classique

public class PostGameManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText; // Ton UI Text
    [SerializeField] private float delayBeforeLoad = 3f;  // Délai en secondes

    public void OnHostWantToReturnLobby(string lobbyName)
    {
        if (!IsHost) return;

        ShowReturnMessageRpc($"Retour au lobby dans {delayBeforeLoad}s...");

        StartCoroutine(ReturnToLobbyWithDelay(lobbyName));
    }

    [Rpc(SendTo.Everyone)]
    private void ShowReturnMessageRpc(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }

    private IEnumerator ReturnToLobbyWithDelay(string lobbyName)
    {
        float timer = delayBeforeLoad;
        while (timer > 0f)
        {
            ShowReturnMessageRpc($"Retour au lobby dans {Mathf.CeilToInt(timer)}s...");
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }
        NetworkManager.Singleton.SceneManager.LoadScene(lobbyName, LoadSceneMode.Single);
    }
}