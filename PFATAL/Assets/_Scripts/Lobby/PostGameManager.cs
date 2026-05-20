using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PostGameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float delayBeforeLoad = 3f;
    [SerializeField] private Button restartButton;
    
    private void Start()
    {
        if (messageText != null)
            messageText.text = "Waiting for the host...";
    }

    public void OnHostWantToReturnLobby(string lobbyName)
    {
        if (!NetworkManager.Singleton.IsHost) return;
        restartButton.interactable = false;
        StartCoroutine(ReturnToLobbyWithDelay(lobbyName));
    }

    private IEnumerator ReturnToLobbyWithDelay(string lobbyName)
    {
        float timer = delayBeforeLoad;
        while (timer > 0f)
        {
            if (messageText != null)
                messageText.text = $"Back to the lobby in {Mathf.CeilToInt(timer)}s...";
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            if (client.PlayerObject != null && client.PlayerObject.gameObject.layer == 8)
                client.PlayerObject.Despawn(true);
        
        restartButton.interactable = true;
        NetworkManager.Singleton.SceneManager.LoadScene(lobbyName, LoadSceneMode.Single);
    }
}