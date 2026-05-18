using NaughtyAttributes;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HeatmapDownloader : MonoBehaviour
{
    private string baseURL = "http://localhost:5000";

    [SerializeField] private int gameIdToDownload = 1;

    [Button]
    public void DownloadHeatmap()
    {
        StartCoroutine(DownloadHeatmapCoroutine(gameIdToDownload));
    }

    public IEnumerator DownloadHeatmapCoroutine(int gameId, Action<HeatMapData> onResult = null)
    {
        string url = baseURL + $"/game/{gameId}/heatmap";

        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[HeatmapDownloader] Erreur HTTP {request.responseCode} : {request.error}");
            onResult?.Invoke(null);
            yield break;
        }

        // Parse du JSON pour extraire la string Base64
        string json = request.downloadHandler.text;
        int start = json.IndexOf("\"heatmap\":\"") + 11;
        int end = json.LastIndexOf("\"");

        if (start < 11 || end <= start)
        {
            Debug.LogError("[HeatmapDownloader] Impossible de parser la heatmap dans la réponse JSON.");
            onResult?.Invoke(null);
            yield break;
        }

        string base64 = json.Substring(start, end - start);

        // Décodage Base64 -> bytes
        byte[] heatmapBytes;
        try
        {
            heatmapBytes = Convert.FromBase64String(base64);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[HeatmapDownloader] Erreur décodage Base64 : {ex.Message}");
            onResult?.Invoke(null);
            yield break;
        }

        // Sauvegarde du .bin (même format que HeatMapServerAnalitics)
        string folder = Path.Combine(Application.persistentDataPath, "HeatMapFolder");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = Path.Combine(folder, $"heatmap_game_{gameId}.bin");
        try
        {
            File.WriteAllBytes(filePath, heatmapBytes);
            Debug.Log($"[HeatmapDownloader] Heatmap sauvegardée ({heatmapBytes.Length} bytes) -> {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[HeatmapDownloader] Erreur écriture fichier : {ex.Message}");
            onResult?.Invoke(null);
            yield break;
        }

        // Désérialisation en HeatMapData via HeatMapUtility
        HeatMapData heatMapData = HeatMapUtility.ConvertByteToMap(heatmapBytes);
        if (heatMapData == null)
        {
            Debug.LogError("[HeatmapDownloader] Echec désérialisation HeatMapData.");
            onResult?.Invoke(null);
            yield break;
        }

        Debug.Log($"[HeatmapDownloader] HeatMapData désérialisé : {heatMapData.points.Count} points.");
        onResult?.Invoke(heatMapData);
    }
}