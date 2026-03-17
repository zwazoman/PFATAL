using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }

    public void LeaveLobby(string sceneName)
    {
        try
        {
            Debug.Log("Leaving lobby...");
            LobbyService.Instance.RemovePlayerAsync(LobbyManager.Instance.GetCurrentLobby().Id, UnityServicesManager.Instance.GetPlayerId());
            _ = NetworkConnectionManager.Instance.Disconnect();
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }
}
