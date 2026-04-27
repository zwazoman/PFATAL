using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseRequest : MonoBehaviour
{
    private string baseURL = "http://localhost:5000";

    //public Player playerTest;
    //public Game gameTest;
    //public List<GamePlayerSet> gamePlayerSetTest;
    //public List<Score> scoreTest;
    //public List<Death> deathTest;


    /*[Button]
    public void AddPlayer()
    {
        StartCoroutine(SendPlayer(playerTest));
    }*/

    /// <summary>
    /// Fonction qui envoie un joueur � la base de donn�es via une requete POST
    /// </summary>
    /// <param name="player"></param>
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

    /*[Button]
    public void AddGame()
    {
        StartCoroutine(SendGame(gameTest,(result) =>
        { 
            Debug.Log("ID de la partie ajout�e : " + result);
        }));
    }*/

    /// <summary>
    /// Fonction qui envoie une partie � la base de donn�es via une requete POST
    /// </summary>
    /// <param name="game"></param>
    public IEnumerator SendGame(Game game, Action <string> onResult)
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
        {
            Debug.LogError(request.error);

            onResult?.Invoke(null);
        }
        else
        {
            Debug.Log("Player sent : " + request.downloadHandler.text);

            onResult?.Invoke(request.downloadHandler.text);
        }
    }

    /*[Button]
    public void AddGamePlayerSet()
    {
        StartCoroutine(SendGamePlayerSet(gamePlayerSetTest));
    }*/

    /// <summary>
    /// Fonction qui envoie une liste de GamePlayerSet � la base de donn�es via une requete POST
    /// </summary>
    /// <param name="gamePlayerSets"></param>
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
        {
            Debug.LogError($"Erreur HTTP {request.responseCode}: {request.downloadHandler.text}");
        }
        else
            Debug.Log("Player sent : " + request.downloadHandler.text);
    }

    /*[Button]
    public void AddScore()
    {
        StartCoroutine(SendScore(scoreTest));
    }*/

    /// <summary>
    /// Fonction qui envoie une liste de Score � la base de donn�es via une requete POST
    /// </summary>
    /// <param name="scores"></param>
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

    /*[Button]
    public void AddDeath()
    {
        StartCoroutine(SendDeath(deathTest));
    }*/

    /// <summary>
    /// Fonction qui envoie une liste de Death � la base de donn�es via une requete POST
    /// </summary>
    /// <param name="deaths"></param>
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
        {
            Debug.LogError($"Erreur HTTP {request.responseCode}: {request.downloadHandler.text}");
        }
        else
            Debug.Log("Player sent : " + request.downloadHandler.text);
    }

    /*[Button]
    public void GetLastGameId()
    {
        StartCoroutine(LastGameIdCoroutine((id) =>
        {
            Debug.Log("ID r�cup�r� : " + id);
        }));
    }*/

    /// <summary>
    /// Fonction qui r�cup�re le dernier ID de partie enregistr� dans la base de donn�es via une requete GET.
    /// </summary>
    /// <param name="onResult"></param>
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
    public long Id;
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
    public long IdPlayer;
}

[Serializable]
public class Score
{
    //public int Id;
    public int IdGame;
    public long IdPlayer;
    public int Points;
}

[Serializable]
public class Death
{
    //public int Id;
    public long VictimId;
    public long KillerId;
    public int Weapon;
    public float Distance;
    public int IdGame;
    public float Time;
}