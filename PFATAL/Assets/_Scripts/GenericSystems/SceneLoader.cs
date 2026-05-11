using DG.Tweening;
using JetBrains.Annotations;
using System;
using Unity.Services.Lobbies;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public event Action OnSceneLoaded;

    [Header("Transition References")]
    [SerializeField] Image _loadingScreen;

    [Header("Transition Settings")]
    [SerializeField] float _fadeDuration = .5f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        OnSceneLoaded += SceneLoaded_Callback;
    }

    public void HardLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        _loadingScreen.gameObject.SetActive(true);
        _loadingScreen.DOFade(1, _fadeDuration).onComplete += () => FadeInEnded_Callback(sceneName);
    }

    void FadeInEnded_Callback(string sceneName)
    {
        Load(sceneName);
        //throbber de con
    }

    void FadeOutEnded_Callback()
    {
        _loadingScreen.gameObject.SetActive(false);
    }

    void SceneLoaded_Callback()
    {
        _loadingScreen.DOFade(0, _fadeDuration).onComplete += FadeOutEnded_Callback;
    }


    async void Load(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Single);
        OnSceneLoaded?.Invoke();
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
