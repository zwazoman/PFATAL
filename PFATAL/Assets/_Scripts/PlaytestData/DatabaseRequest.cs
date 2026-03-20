using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class DatabaseRequest : MonoBehaviour
{
    private string baseURL = "http://localhost:5000";

    public Player playerTest;
    public Game gameTest;
    public List<GamePlayerSet> gamePlayerSetTest;
    public List<Score> scoreTest;
    public List<Death> deathTest;


    [Button]
    public void AddPlayer()
    {
        StartCoroutine(SendPlayer(playerTest));
    }

    public IEnumerator SendPlayer(Player player)
    {
        string url = baseURL + "/player/add";

        string json = JsonUtility.ToJson(player);

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError(request.error);
        else
            Debug.Log("Player sent : " + request.downloadHandler.text);
    }

    [Button]
    public void AddGame()
    {
        StartCoroutine(SendGame(gameTest));
    }

    public IEnumerator SendGame(Game game)
    {
        string url = baseURL + "/game/add";

        string json = JsonUtility.ToJson(game);

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError(request.error);
        else
            Debug.Log("Player sent : " + request.downloadHandler.text);
    }

    [Button]
    public void AddGamePlayerSet()
    {
        StartCoroutine(SendGamePlayerSet(gamePlayerSetTest));
    }

    public IEnumerator SendGamePlayerSet(List<GamePlayerSet> gamePlayerSets)
    {
        string url = baseURL + "/gameplayerset/add";

        GamePlayerSetBatch batch = new GamePlayerSetBatch();
        batch.GamePlayerSets = gamePlayerSets;

        string json = JsonUtility.ToJson(batch);

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError(request.error);
        else
            Debug.Log("Player sent : " + request.downloadHandler.text);
    }

    [Button]
    public void AddScore()
    {
        StartCoroutine(SendScore(scoreTest));
    }

    public IEnumerator SendScore(List<Score> scores)
    {
        string url = baseURL + "/score/add";

        ScoreBatch batch = new ScoreBatch();
        batch.Scores = scores;

        string json = JsonUtility.ToJson(batch);

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError(request.error);
        else
            Debug.Log("Player sent : " + request.downloadHandler.text);
    }

    [Button]
    public void AddDeath()
    {
        StartCoroutine(SendDeath(deathTest));
    }

    public IEnumerator SendDeath(List<Death> deaths)
    {
        string url = baseURL + "/death/add";

        DeathBatch batch = new DeathBatch();
        batch.Deaths = deaths;

        string json = JsonUtility.ToJson(batch);

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError(request.error);
        else
            Debug.Log("Player sent : " + request.downloadHandler.text);
    }

    [Button]
    public void GetLastGameId()
    {
        StartCoroutine(LastGameIdCoroutine((id) =>
        {
            Debug.Log("ID récupéré : " + id);
        }));
    }

    public IEnumerator LastGameIdCoroutine(Action<int> onResult)
    {
        string url = baseURL + "/game/lastid";

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string text = request.downloadHandler.text.Replace("}", "");
            string[] content = text.Split(":");
            int id = int.Parse(content[1]);

            Debug.Log("SQL RESULT : " + id);

            onResult?.Invoke(id);
        }
        else
        {
            Debug.LogError("SQL ERROR : " + request.error);

            onResult?.Invoke(-1);
        }
    }
}

[Serializable]
public class GamePlayerSetBatch
{
    public List<GamePlayerSet> GamePlayerSets;
}

[Serializable]
public class ScoreBatch
{
    public List<Score> Scores;
}

[Serializable]
public class DeathBatch
{
    public List<Death> Deaths;
}

[Serializable]
public class Player
{
    //public int Id;
    public string Name;
}

[Serializable]
public class Game
{
    //public int Id;
    public int Version;
    public int IdPlayerSet;
    public string HeatMap;
    public int GameMode;
    public string MapName;
    public float Duration;
}

[Serializable]
public class GamePlayerSet
{
    public int Id;
    public int IdGame;
    public int IdPlayer;
}

[Serializable]
public class Score
{
    //public int Id;
    public int IdGame;
    public int IdPlayer;
    public int Points;
}

[Serializable]
public class Death
{
    //public int Id;
    public int VictimId;
    public int KillerId;
    public int Weapon;
    public float Distance;
    public int IdGame;
    public float Time;
}