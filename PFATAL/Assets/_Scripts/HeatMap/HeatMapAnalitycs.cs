using UnityEngine;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using System.Collections.Generic;
using System;

public class HeatMapAnalitycs : MonoBehaviour
{
    public float UpdateInterval = 1f;
    public float gridSize = 1f;
    public Transform playerTransform;

    private HeatMapData heatMapData;
    private float timer;
    private string filePath;

    [Header("Debug")]
    [SerializeField] private bool show = false;
    [SerializeField] private List<string> listeFile;
    [SerializeField] private List<string> listeHeatmapJson;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "heatmap.json");
        LoadHeatMap();

        GUIUtility.systemCopyBuffer = Application.persistentDataPath;
    }

    private void Update()
    {
        if ((timer += Time.deltaTime) >= UpdateInterval)
        {
            timer = 0f;
            Vector3 playerPos = playerTransform.position;

            var point = heatMapData.points.FirstOrDefault(p => 
                p.x == Mathf.Round(playerPos.x / gridSize) * gridSize &&
                p.y == Mathf.Round(playerPos.y / gridSize) * gridSize &&
                p.z == Mathf.Round(playerPos.z / gridSize) * gridSize);

            if (point != null)
                point.visitsGlobal++;
            else
                heatMapData.points.Add(new HeatPoint(
                    Mathf.Round(playerPos.x / gridSize) * gridSize,
                    Mathf.Round(playerPos.y / gridSize) * gridSize,
                    Mathf.Round(playerPos.z / gridSize) * gridSize));

            File.WriteAllText(filePath, JsonUtility.ToJson(heatMapData));
        }

        Debug.Log($"Max Visits: {MaxVisits()}");
    }

    private HeatMapData LoadHeatMap()
    {
        heatMapData = File.Exists(filePath) 
            ? JsonUtility.FromJson<HeatMapData>(File.ReadAllText(filePath)) 
            : new HeatMapData(gridSize);

        Debug.Log(heatMapData);

        return heatMapData;
    }

    private void OnApplicationQuit()
    {
        File.WriteAllText(filePath, JsonUtility.ToJson(heatMapData));
    }

    public void OnDrawGizmos()
    {

        if (heatMapData == null) return;


        if (!show) return;

        float size = heatMapData.heatMapCellSize;
        foreach (var point in heatMapData.points)
        {
            //Gizmos.color = Color.Lerp(Color.blue, Color.red, point.visits / MaxVisits());
            Debug.Log(point.visitsGlobal / MaxVisits());
            Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)point.visitsGlobal / 10);
            Gizmos.DrawCube(new Vector3(point.x, point.y, point.z), new Vector3(size, size, size));
        }
    }

    public int MaxVisits()
    {
        return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.visitsGlobal) : 0;
    }

    [Button("DEGAGE LE JSON")]
    public void DeleteHeatMap()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, "heatmap.json"));
    }

    [Button("Coppy file path")]
    public void copy()
    {
        GUIUtility.systemCopyBuffer = Application.persistentDataPath;
    }

    [Button("Send HeatMap")]
    public void SendHeatMapFunc()
    {
        string path = Path.Combine(Application.persistentDataPath, "heatmap.json");
        string jsonData;

        if (!File.Exists(path)) return;

        jsonData = File.ReadAllText(path);

        GetComponent<SendHeatMap>().SendJsonData(jsonData + "  C'EST L'HEURE, connard.  " + System.DateTime.Now);

        //SendHeatMap.SendJsonData(jsonData + "  C'EST L'HEURE, connard. Inchallah ça marche zebi  " + System.DateTime.Now);
        
        Debug.Log("HeatMap sent!");
    }

    [Button("Combine HeatMaps")]
    public void Combine()
    {
        listeFile = Directory.GetFiles(Application.persistentDataPath, "heatmap_*.json").ToList();

        foreach (string file in listeFile)
            listeHeatmapJson.Add(File.ReadAllText(file));

        Debug.Log("COMBINE");

        List<HeatMapData> heatMaps = HeatMapUtility.ConvertJsonListToHeatMapDataList(listeHeatmapJson);

        if (!HeatMapUtility.IsSameGridSize(heatMaps)) return;

        string finalHeatMap = HeatMapUtility.CombineHeatMap(heatMaps);

        File.WriteAllText(Path.Combine(Application.persistentDataPath, "heatmap_combined.json"), finalHeatMap);
    }
}