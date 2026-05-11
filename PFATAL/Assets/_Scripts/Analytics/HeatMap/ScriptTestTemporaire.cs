using NaughtyAttributes;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScriptTestTemporaire : MonoBehaviour
{
    public string path;
    public bool show = false;
    [Range(0, 50)]
    public int minMaxVisits = 10;

    public Texture3D texture;
    public Vector3Int pixelPos;

    public List<float> colorRed = new();


    public string heatMapPath;
    public WeaponType weaponType;
    public int cellSize;
    public int gameId;
    public int gameVersion;
    public int playerId;

    public void OnDrawGizmos()
    {
        if (!File.Exists(path)) return;
        if (!show) return;


        HeatMapData heatMapData = HeatMapUtility.ConvertByteToMap(File.ReadAllBytes(path));
        cellSize = heatMapData.cellSize;
        gameId = heatMapData.gameId;
        gameVersion = heatMapData.gameVersion;
        playerId = heatMapData.playerId;

        float size = heatMapData.cellSize;
        foreach (var point in heatMapData.points)
        {
            //Gizmos.color = Color.Lerp(Color.blue, Color.red, point.visits / MaxVisits());
            Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)point.GetGlobalVisits() / minMaxVisits);
            Gizmos.DrawWireCube(new Vector3(point.P[0], point.P[1], point.P[2]), new Vector3(size, size, size));

        }
    }

    [Button("Test pour voir les valeur de dégradé")]
    public void GetPixel()
    {
        HeatMapData heatMapData = HeatMapUtility.ConvertByteToMap(File.ReadAllBytes(path));

        float size = heatMapData.cellSize;
        foreach (var point in heatMapData.points)
        {
            if (point.P[1] == 17 & point.P[2] == 23)
            {
                Debug.Log($"Color : {Color.Lerp(Color.blue, Color.red, (float)point.GetGlobalVisits() / minMaxVisits)}, T : {(float)point.GetGlobalVisits() / minMaxVisits}");
            }
        }
    }

    [Button("Test GetMaxVisits")]
    public void GetMaxTest()
    {
        HeatMapData heatMap = JsonUtility.FromJson<HeatMapData>(File.ReadAllText(Path.Combine(Application.persistentDataPath, path)));

        HeatMapUtility.MaxVisits(heatMap, weaponType);
    }
}