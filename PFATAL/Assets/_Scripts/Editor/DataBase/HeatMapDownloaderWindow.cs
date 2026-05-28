using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class HeatmapDownloaderWindow : EditorWindow
{
    private string baseURL = "http://10.84.108.96:5000";

    private int gameIdToDownload = 1;

    [MenuItem("Window/HeatmapDownloaderWindow")]
    public static void ShowWindow()
    {
        GetWindow<HeatmapDownloaderWindow>("Heatmap Downloader");
    }

    void OnGUI()
    {
        baseURL = EditorGUILayout.TextField("Base URL", baseURL);
        gameIdToDownload = EditorGUILayout.IntField("Game ID to Download", gameIdToDownload);

        if (GUILayout.Button("Download Heatmap"))
        {
            DownloadHeatmap(gameIdToDownload);
        }
    }

    public async void DownloadHeatmap(int gameId, Action<HeatMapData> onResult = null)
    {
        string url = baseURL + $"/game/{gameId}/heatmap";

        using UnityWebRequest request = UnityWebRequest.Get(url);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[HeatmapDownloader] Erreur HTTP {request.responseCode} : {request.error}");
            onResult?.Invoke(null);
            await Task.CompletedTask;
        }

        // Parse du JSON pour extraire la string Base64
        string json = request.downloadHandler.text;
        int start = json.IndexOf("\"heatmap\":\"") + 11;
        int end = json.LastIndexOf("\"");

        if (start < 11 || end <= start)
        {
            Debug.LogError("[HeatmapDownloader] Impossible de parser la heatmap dans la reponse JSON.");
            onResult?.Invoke(null);
            await Task.CompletedTask;
        }

        string base64 = json.Substring(start, end - start);

        // Décodage Base64 -> bytes
        byte[] heatmapBytes = null;
        try
        {
            heatmapBytes = Convert.FromBase64String(base64);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[HeatmapDownloader] Erreur decodage Base64 : {ex.Message}");
            onResult?.Invoke(null);
            await Task.CompletedTask;
        }

        // Sauvegarde du .bin (meme format que HeatMapServerAnalitics)
        string folder = Path.Combine(Application.persistentDataPath, "HeatMapFolder");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = Path.Combine(folder, $"heatmap_game_{gameId}.bin");
        try
        {
            File.WriteAllBytes(filePath, heatmapBytes);
            Debug.Log($"[HeatmapDownloader] Heatmap sauvegard?e ({heatmapBytes.Length} bytes) -> {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[HeatmapDownloader] Erreur ecriture fichier : {ex.Message}");
            onResult?.Invoke(null);
            await Task.CompletedTask;
        }

        // Deserialisation en HeatMapData via HeatMapUtility
        HeatMapData heatMapData = HeatMapUtility.ConvertByteToMap(heatmapBytes);
        if (heatMapData == null)
        {
            Debug.LogError("[HeatmapDownloader] Echec deserialisation HeatMapData.");
            onResult?.Invoke(null);
            await Task.CompletedTask;
        }

        Debug.Log($"[HeatmapDownloader] HeatMapData deserialise : {heatMapData.points.Count} points.");
        onResult?.Invoke(heatMapData);
    }
}
