
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeatMapUtility
{
    public static string CombineHeatMap(List<HeatMapData> heatMaps)
    {
        if (!IsSameGridSize(heatMaps)) return null;

        HeatMapData combinedHeatMap = new HeatMapData(heatMaps[0].heatMapCellSize);

        foreach (HeatMapData heatmap in heatMaps)
        {
            for (int i = 0; i < heatmap.points.Count; i++)
            {
                HeatPoint currentPoint = heatmap.points[i];

                var point = combinedHeatMap.points.FirstOrDefault(p => p.x == currentPoint.x && p.y == currentPoint.y && p.z == currentPoint.z);
                
                if (point != null)
                    point.visitsGlobal += currentPoint.visitsGlobal;
                
                else
                    combinedHeatMap.points.Add(new HeatPoint(
                        currentPoint.x,
                        currentPoint.y,
                        currentPoint.z,
                        currentPoint.visitsGlobal
                    ));
            }
        }

        combinedHeatMap.heatMapGameId = heatMaps[0].heatMapGameId;
        combinedHeatMap.heatMapPlayerNumber = heatMaps.Count;
        string combinedHeatMapJson = ConvertHeatMapDataToJson(combinedHeatMap);

        return combinedHeatMapJson;
    }

    public static bool IsSameGridSize(List<HeatMapData> heatMaps)
    {
        float gridSize = heatMaps[0].heatMapCellSize;

        for (int i = 1; i < heatMaps.Count; i++)
        {
            if (heatMaps[i].heatMapCellSize != gridSize)
            {
                Debug.LogError("Heat maps have different grid sizes. Cannot combine.");
                return false;
            }
        }

        return true;
    }

    public static bool IsSameGameId(List<HeatMapData> heatMaps)
    {
        int gameId = heatMaps[0].heatMapGameId;
        for (int i = 1; i < heatMaps.Count; i++)
        {
            if (heatMaps[i].heatMapGameId != gameId)
            {
                Debug.LogError("Heat maps have different game IDs. Cannot combine.");
                return false;
            }
        }
        return true;
    }

    public static List<HeatMapData> ConvertJsonListToHeatMapDataList(List<string> heatMapJsonList)
    {
        List<HeatMapData> heatMapDataList = new List<HeatMapData>();
        
        foreach (string heatMapJson in heatMapJsonList)
        {
            heatMapDataList.Add(ConvertJsonToHeatMapData(heatMapJson));
        }

        return heatMapDataList;
    }

    public static List<string> ConvertHeatMapDataListToJsonList(List<HeatMapData> heatMapDataList)
    {
        List<string> heatMapJsonList = new List<string>();
        
        foreach (HeatMapData heatMapData in heatMapDataList)
        {
            heatMapJsonList.Add(ConvertHeatMapDataToJson(heatMapData));
        }
        return heatMapJsonList;
    }

    public static HeatMapData ConvertJsonToHeatMapData(string heatMapJson)
    {
        HeatMapData data = JsonUtility.FromJson<HeatMapData>(heatMapJson);
        return data;
    }

    public static string ConvertHeatMapDataToJson(HeatMapData heatMapData)
    {
        string heatMapJson = JsonUtility.ToJson(heatMapData);
        return heatMapJson;
    }
}
