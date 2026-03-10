using NaughtyAttributes;
using System.Collections;
using System.IO;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Networking;

public class PlaytestDatabase : MonoBehaviour
{
    private SQLiteConnection db;
    private int gameId;

    private string url = "https://drive.google.com/uc?export=download&id=1yvwOUUfHH9x5VAppJQM3LwEpPqk9Po22";
    private string uploadUrl = "https://script.google.com/macros/s/AKfycby_s3TIxznhPVsf-bd3LsvFlGmh2hg83PJ9lD9SEegFrte8Wqg3p778irUWqxx3qw0WHQ/exec";

    [Button]
    public void LoadFromDrive()
    {
        StartCoroutine(DownloadDB());
    }

    IEnumerator DownloadDB()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "PlaytestData.db");
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erreur téléchargement DB: " + www.error);
        }
        else
        {
            File.WriteAllBytes(dbPath, www.downloadHandler.data);
            Debug.Log("DB téléchargée et sauvegardée localement à : " + dbPath);
        }
    }

    [Button]
    public void LinkDataBase()
    {
        string path = Path.Combine(Application.persistentDataPath, "PlaytestData.db");
        db = new SQLiteConnection(path);

        db.CreateTable<PlaySession>();
        db.CreateTable<PositionLog>();
    }

    public void GetLastEntries()
    {
        var logs = db.Table<PositionLog>().OrderByDescending(x => x.Id).Take(10).ToList();

        foreach (var log in logs)
        {
            Debug.Log($"Position Log {log.Id}: Session {log.SessionId} at ({log.X}, {log.Y}, {log.Z}) at time {log.TimeStamp}");
        }
    }

    public void GetLastPlaySessionId()
    {
        PlaySession session = db.Table<PlaySession>().OrderByDescending(x => x.Id).FirstOrDefault();

        gameId = session != null ? session.GameId : 0;
    }

    [Button]
    public void DebugSession()
    {
        CreateSession("player_01", 5, 2, "Rifle", 0.75f);
    }

    public void CreateSession(string playerName, int kills, int deaths, string mainWeapon, float accuracy)
    {
        PlaySession session = new PlaySession
        {
            GameId = gameId,
            PlayerName = playerName,
            Kills = kills,
            Deaths = deaths,
            MainWeapon = mainWeapon,
            Accuracy = accuracy,
            GameVersion = Application.version
        };

        db.Insert(session);
    }

    [Button]
    public void DebugLogPosition()
    {
        LogPosition(new Vector3(0,5,10),2.5f,4);
    }

    public void LogPosition(Vector3 position, float time, int visit)
    {
        PositionLog log = new PositionLog
        {
            SessionId = gameId,
            X = position.x,
            Y = position.y,
            Z = position.z,
            TimeStamp = time,
            Visit = visit,
            GameVersion = Application.version
        };

        db.Insert(log);
    }

    [Button]
    public void Upload()
    {
        StartCoroutine(UploadDatabase());
    }

    IEnumerator UploadDatabase()
    {
        string path = Application.persistentDataPath + "/PlaytestData.db";

        byte[] fileData = File.ReadAllBytes(path);
        string base64 = System.Convert.ToBase64String(fileData);

        WWWForm form = new WWWForm();
        form.AddField("data", base64);

        UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("File ID : " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}