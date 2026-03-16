using AYellowpaper.SerializedCollections.Editor.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that contains some functions for the heatmapData
/// </summary>
public class HeatMapUtility
{
    /// <summary>
    /// Combines multiple heat map data objects into a single aggregated heat map and returns its JSON representation.
    /// </summary>
    /// <remarks>This method aggregates visit counts for each unique point across the provided heat maps. The
    /// combined heat map will use the grid size and game ID from the first heat map in the list. It is important to
    /// ensure that all heat maps share the same grid size to avoid errors.</remarks>
    /// <param name="heatMaps">A list of heat map data objects to be combined. All heat maps must have the same grid size; otherwise, the
    /// method returns null.</param>
    /// <returns>A JSON string representing the combined heat map data. Returns null if the input heat maps do not share the same
    /// grid size.</returns>
    public static string CombineHeatMap(List<HeatMapData> heatMaps)
    {
        //check if the grid size is the same for all the heatmap
        if (!IsSameGridSize(heatMaps))
        {
            Debug.LogError("All the heatmap don't have the same gridSize");
            return null;
        }

        HeatMapData combinedHeatMap = new HeatMapData(heatMaps[0].heatMapCellSize);

        foreach (HeatMapData heatmap in heatMaps)
        {
            for (int i = 0; i < heatmap.points.Count; i++)
            {
                HeatPoint currentPoint = heatmap.points[i];

                //checking if the point already exists in the combinedHeatmap list
                var point = combinedHeatMap.points.FirstOrDefault(p => p.x == currentPoint.x && p.y == currentPoint.y && p.z == currentPoint.z);
                
                //if the point exist, we just add the corresponding value together
                if (point != null)
                {
                    point.visitsGlobal += currentPoint.visitsGlobal; 
                    point.playerWithHammerVisits += currentPoint.playerWithHammerVisits; 
                    point.playerWithCrossbowVisits += currentPoint.playerWithCrossbowVisits;
                    point.playerWithTomahawkVisits += currentPoint.playerWithTomahawkVisits;
                }

                //if the point doesn't exist, we create it and we add it in the list
                else
                    combinedHeatMap.points.Add(new HeatPoint(
                        currentPoint.x,
                        currentPoint.y,
                        currentPoint.z,
                        currentPoint.visitsGlobal,
                        currentPoint.playerWithHammerVisits,
                        currentPoint.playerWithCrossbowVisits,
                        currentPoint.playerWithTomahawkVisits
                    ));
            }
        }

        //set the correct game id, 0 if there's severals game heatmap used
        if (IsSameGameId(heatMaps))
        {
            combinedHeatMap.heatMapGameId = heatMaps[0].heatMapGameId;
        }
        else
        {
            combinedHeatMap.heatMapGameId = 0;
        }

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

    public static int MaxVisits(HeatMapData heatMapData)
    {
        return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.visitsGlobal) : 0;
    }
}
