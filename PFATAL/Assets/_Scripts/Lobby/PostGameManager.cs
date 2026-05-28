using System.Collections;
using System.Collections.Generic;
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

        // Snapshot de la liste AVANT d'itérer
        var objectsToDespawn = new List<NetworkObject>(
            NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
        );

        foreach (NetworkObject netObj in objectsToDespawn)
        {
            if (netObj == null) continue;
            int layer = netObj.gameObject.layer;
            if (layer == 8 || layer == 7)
                netObj.Despawn(true);
        }   
        
        restartButton.interactable = true;
        NetworkManager.Singleton.SceneManager.LoadScene(lobbyName, LoadSceneMode.Single);
    }
}