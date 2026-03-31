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

    public void OnDrawGizmos()
    {
        if (!File.Exists(path)) return;
        if (!show) return;


        HeatMapData heatMapData = JsonUtility.FromJson<HeatMapData>(File.ReadAllText(path));

        float size = heatMapData.heatMapCellSize;
        foreach (var point in heatMapData.points)
        {
            //Gizmos.color = Color.Lerp(Color.blue, Color.red, point.visits / MaxVisits());
            Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)point.visitsGlobal / minMaxVisits);
            Gizmos.DrawWireCube(new Vector3(point.x, point.y, point.z), new Vector3(size, size, size));
            
        }
    }

    [Button("Test pour voir les valeur de dégradé")]
    public void GetTheFuckingPixel()
    {
        HeatMapData heatMapData = JsonUtility.FromJson<HeatMapData>(File.ReadAllText(path));

        float size = heatMapData.heatMapCellSize;
        foreach (var point in heatMapData.points)
        {
            if (point.y == 17 & point.z == 23)
            {
                Debug.Log($"Color : {Color.Lerp(Color.blue, Color.red, (float)point.visitsGlobal / minMaxVisits)}, T : {(float)point.visitsGlobal / minMaxVisits}");
            }
        }
    }
}
