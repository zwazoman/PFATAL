
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

    /// <summary>
    /// Determines whether all heat maps in the specified list use the same grid cell size.
    /// </summary>
    /// <remarks>If the grid cell sizes differ, an error message is logged and the heat maps cannot be
    /// combined.</remarks>
    /// <param name="heatMaps">A list of <see cref="HeatMapData"/> objects to check for consistent grid cell sizes. The list must contain at
    /// least one element.</param>
    /// <returns>Returns <see langword="true"/> if all heat maps have the same grid cell size; otherwise, <see
    /// langword="false"/>.</returns>
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

    /// <summary>
    /// Check if all the heatmap in the list came from the same game.
    /// </summary>
    /// <remarks>
    /// If the game id are differents, an error message is logged.
    /// </remarks>
    /// <param name="heatMaps">A list of <see cref="HeatMapData"/> objects to check for same game id.</param>
    /// <returns>Returns <see langword="true"/> if all the heatmap are from the same game, else returns <see langword="false">.</returns>
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

    /// <summary>
    /// Converts a list of JSON strings representing heat map data into a list of HeatMapData objects.
    /// </summary>
    /// <remarks>Each JSON string in the input list must be properly formatted to match the expected structure
    /// for conversion. If any string is not valid JSON, it may result in an exception during conversion.</remarks>
    /// <param name="heatMapJsonList">A list of JSON strings, where each string represents a heat map data entry to be converted.</param>
    /// <returns>A list of HeatMapData objects created from the provided JSON strings.</returns>
    public static List<HeatMapData> ConvertJsonListToHeatMapDataList(List<string> heatMapJsonList)
    {
        List<HeatMapData> heatMapDataList = new List<HeatMapData>();
        
        foreach (string heatMapJson in heatMapJsonList)
        {
            heatMapDataList.Add(ConvertJsonToHeatMapData(heatMapJson));
        }

        return heatMapDataList;
    }

    /// <summary>
    /// Converts a collection of HeatMapData objects to their JSON string representations.
    /// </summary>
    /// <remarks>This method iterates through the provided list and serializes each HeatMapData object using
    /// the ConvertHeatMapDataToJson method. The order of the resulting JSON strings matches the order of the input
    /// objects.</remarks>
    /// <param name="heatMapDataList">A list of HeatMapData objects to be serialized. Each object must be properly initialized and not null.</param>
    /// <returns>A list of JSON strings, where each string represents a corresponding HeatMapData object from the input list.</returns>
    public static List<string> ConvertHeatMapDataListToJsonList(List<HeatMapData> heatMapDataList)
    {
        List<string> heatMapJsonList = new List<string>();
        
        foreach (HeatMapData heatMapData in heatMapDataList)
        {
            heatMapJsonList.Add(ConvertHeatMapDataToJson(heatMapData));
        }
        return heatMapJsonList;
    }

    /// <summary>
    /// Converts a JSON string representation of heat map data into a HeatMapData object.
    /// </summary>
    /// <remarks>Ensure that the input JSON string adheres to the expected structure of HeatMapData;
    /// otherwise, the conversion may fail.</remarks>
    /// <param name="heatMapJson">The JSON string that contains the heat map data to be converted. It must be a valid JSON format representing the
    /// HeatMapData structure.</param>
    /// <returns>A HeatMapData object populated with the data parsed from the provided JSON string.</returns>
    public static HeatMapData ConvertJsonToHeatMapData(string heatMapJson)
    {
        HeatMapData data = JsonUtility.FromJson<HeatMapData>(heatMapJson);
        return data;
    }

    /// <summary>
    /// Converts the specified HeatMapData object into its JSON representation.
    /// </summary>
    /// <param name="heatMapData">The HeatMapData object to be converted to JSON. This object must not be null.</param>
    /// <returns>A string containing the JSON representation of the provided HeatMapData object.</returns>
    public static string ConvertHeatMapDataToJson(HeatMapData heatMapData)
    {
        string heatMapJson = JsonUtility.ToJson(heatMapData);
        return heatMapJson;
    }

    /// <summary>
    /// Get the max number of visits in the heatmap.
    /// </summary>
    /// <param name="heatMapData">The heatmap to search maximum visits.</param>
    /// <returns>Returns the maximum visits.</returns>
    public static int MaxVisits(HeatMapData heatMapData, WeaponType weaponType)
    {
        switch (weaponType)
        {
            //global
            case WeaponType.All:
                return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.visitsGlobal) : 0;
            
            //hammer
            case WeaponType.Hammer:
                return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.playerWithHammerVisits) : 0;
            
            //crosbow
            case WeaponType.Crossbow:
                return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.playerWithCrossbowVisits) : 0;

            //tomahawk
            case WeaponType.Tomahawk:
                return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.playerWithTomahawkVisits) : 0;
            
        }

        Debug.LogError("Cassé");
        return 0;
    }
}
