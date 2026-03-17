using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using NaughtyAttributes;

public class SQLQueryAPI : MonoBehaviour
{
    private string baseURL = "http://localhost:5000";
    public string sqlCommand = "SELECT * FROM Player";

    [Button]
    public void SendQuery()
    {
        StartCoroutine(QueryCoroutine(sqlCommand));
    }

    IEnumerator QueryCoroutine(string Command)
    {
        string encoded = UnityWebRequest.EscapeURL(Command);

        string url = baseURL + "/query?cmd=" + encoded;

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("SQL RESULT : " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("SQL ERROR : " + request.error);
        }
    }
}