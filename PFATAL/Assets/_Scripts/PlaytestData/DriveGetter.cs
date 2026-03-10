using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DriveGetter : MonoBehaviour
{
    private string url = "https://script.google.com/macros/s/AKfycbwN9ekgAgowrdznVBl6Ec8kKhmwTKqnU4Gb2xrKWvlM86g0cv8GPDD1XzwHhruhDECzFg/exec";
    public string JsonData { get; private set; }

    [Button]
    public void LoadFromDrive()
    {
        StartCoroutine(GetRequest());
    }

    IEnumerator GetRequest()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Données reçues : " + request.downloadHandler.text);
            JsonData = request.downloadHandler.text;
        }
        else
        {
            Debug.LogError("Erreur : " + request.error);
        }
    }
}
