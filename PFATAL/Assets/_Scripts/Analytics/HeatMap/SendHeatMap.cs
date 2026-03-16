using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SendHeatMap : MonoBehaviour
{
    public void SendJsonData(string json)
    {
        StartCoroutine(SendData(json));
    }

    IEnumerator SendData(string json)
    {
        string url = "https://script.google.com/macros/s/AKfycbzdu6iNzx-kKahJiUeRNFZfq4PCRr36BFm3kB4veD3pHHDuZEq3GGaoBYQbB12zcN_SXQ/exec";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log(request.downloadHandler.text);
    }
}
