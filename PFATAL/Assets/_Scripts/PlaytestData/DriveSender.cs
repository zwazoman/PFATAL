using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DriveSender : MonoBehaviour
{
    private string url = "https://script.google.com/macros/s/AKfycbwN9ekgAgowrdznVBl6Ec8kKhmwTKqnU4Gb2xrKWvlM86g0cv8GPDD1XzwHhruhDECzFg/exec";

    public void SendData(string Data)
    {
        string json = Data;
        StartCoroutine(PostRequest(json));
    }

    IEnumerator PostRequest(string json)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Succès : " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Erreur : " + request.error);
        }
    }
}
