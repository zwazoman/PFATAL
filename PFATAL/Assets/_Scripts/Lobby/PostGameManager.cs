using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PostGameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float delayBeforeLoad = 3f;

    [Rpc(SendTo.Everyone)]
    private void ShowReturnMessageRpc(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }

    public void OnHostWantToReturnLobby(string lobbyName)
    {
        Debug.Log($"OnHostWantToReturnLobby called. IsHost={NetworkManager.Singleton.IsHost}, IsSpawned={NetworkManager.Singleton.IsHost}");
    
        if (!NetworkManager.Singleton.IsHost) return;
    
        Debug.Log("Starting coroutine...");
        StartCoroutine(ReturnToLobbyWithDelay(lobbyName));
    }

    private IEnumerator ReturnToLobbyWithDelay(string lobbyName)
    {
        float timer = delayBeforeLoad;
        while (timer > 0f)
        {
            Debug.Log($"Countdown: {Mathf.CeilToInt(timer)}");
            ShowReturnMessageRpc($"Retour au lobby dans {Mathf.CeilToInt(timer)}s...");
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        // Détruire les player objects sur le réseau avant de changer de scène
        foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj.layer == 8)
            {
                NetworkObject netObj = obj.GetComponent<NetworkObject>();
                if (netObj != null)
                    netObj.Despawn(true);
                else
                    Destroy(obj);
            }
        }
        
        Debug.Log($"Calling LoadScene: {lobbyName}");
        NetworkManager.Singleton.SceneManager.LoadScene(lobbyName, LoadSceneMode.Single);
    }
}