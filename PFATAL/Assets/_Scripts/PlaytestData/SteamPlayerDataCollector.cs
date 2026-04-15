using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Steamworks;

public class SteamPlayerDataCollector : MonoBehaviour
{
    [SerializeField] private DatabaseRequest _databaseRequest;
    private string _apiBaseUrl = "http://localhost:5000";

    private void OnEnable()
    {
        SteamAuthenticator.OnAuthSuccess += HandleAuthSuccess;
    }

    private void OnDisable()
    {
        SteamAuthenticator.OnAuthSuccess -= HandleAuthSuccess;
    }

    private void HandleAuthSuccess(CSteamID steamID, string playerName)
    {
        StartCoroutine(CheckAndRegisterPlayer(steamID, playerName));
    }

    private IEnumerator CheckAndRegisterPlayer(CSteamID steamID, string playerName)
    {
        long id = (long)steamID.m_SteamID;
        string url = $"{_apiBaseUrl}/player/exists/{id}";

        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[SteamPlayerDataCollector] Erreur vérification joueur : {request.error}");
            yield break;
        }

        var json = request.downloadHandler.text;

        bool exists = json.Contains("\"exists\":true");

        if (exists)
        {
            Debug.Log($"[SteamPlayerDataCollector] Joueur {id} déjà dans le DB");
            yield break;
        }

        Player player = new Player
        {
            Id = (long)id,
            Name = playerName
        };

        StartCoroutine(_databaseRequest.SendPlayer(player));
        Debug.Log($"[SteamPlayerDataCollector] Nouveau joueur enregistré : {id} | {playerName}");
    }
}