using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.LightTransport;
using static UnityEngine.Rendering.DebugUI;

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

        HeatMapData combinedHeatMap = new HeatMapData(heatMaps[0].cellSize);

        foreach (HeatMapData heatmap in heatMaps)
        {
            for (int i = 0; i < heatmap.points.Count; i++)
            {
                HeatPoint currentPoint = heatmap.points[i];

                //checking if the point already exists in the combinedHeatmap list
                var point = combinedHeatMap.points.FirstOrDefault(p => p.P == currentPoint.P);
                
                //if the point exist, we just add the corresponding value together
                if (point != null)
                {
                    //point.G += currentPoint.G; 
                    point.H += currentPoint.H; 
                    point.C += currentPoint.C;
                    point.T += currentPoint.T;
                }

                //if the point doesn't exist, we create it and we add it in the list
                else
                    combinedHeatMap.points.Add(new HeatPoint(
                        currentPoint.P,
                        //currentPoint.G,
                        point.W,
                        currentPoint.H,
                        currentPoint.C,
                        currentPoint.T
                    ));
            }
        }

        //set the correct game id, 0 if there's severals game heatmap used
        if (IsSameGameId(heatMaps))
        {
            combinedHeatMap.gameId = heatMaps[0].gameId;
        }
        else
        {
            combinedHeatMap.gameId = 0;
        }

        combinedHeatMap.playerCount = heatMaps.Count;
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
        float gridSize = heatMaps[0].cellSize;

        for (int i = 1; i < heatMaps.Count; i++)
        {
            if (heatMaps[i].cellSize != gridSize)
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
        int gameId = heatMaps[0].gameId;
        for (int i = 1; i < heatMaps.Count; i++)
        {
            if (heatMaps[i].gameId != gameId)
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
                return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.W + p.H + p.C + p.T) : 0;

            case WeaponType.Without:
                return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.W) : 0;

            //hammer
            case WeaponType.Hammer:
                return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.H) : 0;
            
            //crosbow
            case WeaponType.Crossbow:
                return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.C) : 0;

            //tomahawk
            case WeaponType.Tomahawk:
                return heatMapData.points.Count > 0 ? heatMapData.points.Max(p => p.T) : 0;
            
        }

        Debug.LogError("Cassé");
        return 0;
    }

    public static byte[] ConvertMapToByte(HeatMapData heatMapData)
    {
        if (heatMapData == null)
        {
            Debug.LogError("Tu te fou de ma gueule, connard");
            return null;
        }

        int index = 0;
        byte[] mapInfo = new byte[5 * sizeof(short) + heatMapData.points.Count * 7 * sizeof(short)]; // Assuming HeatMapData has 5 integer properties (cellSize, gameId, gameVersion, playerCount) and 1 for points count
        Debug.Log($"{sizeof(short)}");

        // Convert HeatMapData properties to bytes
        Buffer.BlockCopy(BitConverter.GetBytes((short)heatMapData.cellSize), 0, mapInfo, index, 2);
        index += 2;
        Buffer.BlockCopy(BitConverter.GetBytes((short)heatMapData.gameId), 0, mapInfo, index, 2);
        index += 2;
        Buffer.BlockCopy(BitConverter.GetBytes((short)heatMapData.gameVersion), 0, mapInfo, index, 2);
        index += 2;
        Buffer.BlockCopy(BitConverter.GetBytes((short)heatMapData.playerCount), 0, mapInfo, index, 2);
        index += 2;
        Buffer.BlockCopy(BitConverter.GetBytes((short)heatMapData.points.Count), 0, mapInfo, index, 2);
        index += 2;
        // Convert HeatPoint properties to bytes
        foreach (var point in heatMapData.points)
        {
            Buffer.BlockCopy(BitConverter.GetBytes((short)point.P[0]), 0, mapInfo, index, 2);
            index += 2;

            Buffer.BlockCopy(BitConverter.GetBytes((short)point.P[1]), 0, mapInfo, index, 2);
            index += 2;

            Buffer.BlockCopy(BitConverter.GetBytes((short)point.P[2]), 0, mapInfo, index, 2);
            index += 2;

            Buffer.BlockCopy(BitConverter.GetBytes((short)point.W), 0, mapInfo, index, 2);
            index += 2;

            Buffer.BlockCopy(BitConverter.GetBytes((short)point.H), 0, mapInfo, index, 2);
            index += 2;

            Buffer.BlockCopy(BitConverter.GetBytes((short)point.C), 0, mapInfo, index, 2);
            index += 2;

            Buffer.BlockCopy(BitConverter.GetBytes((short)point.T), 0, mapInfo, index, 2);
            index += 2;
        }

        #region TEST
        byte[] test = new byte[(5 + 7)*4];
        test[0] = BitConverter.GetBytes(heatMapData.cellSize)[0];
        test[4] = BitConverter.GetBytes(heatMapData.gameId)[0];
        test[8] = BitConverter.GetBytes(heatMapData.gameVersion)[0];
        test[12] = BitConverter.GetBytes(heatMapData.playerCount)[0];
        test[16] = BitConverter.GetBytes(heatMapData.points.Count)[0];
        test[20] = BitConverter.GetBytes(heatMapData.points[0].P[0])[0];
        test[24] = BitConverter.GetBytes(heatMapData.points[0].P[1])[0];
        test[28] = BitConverter.GetBytes(heatMapData.points[0].P[2])[0];
        test[32] = BitConverter.GetBytes(heatMapData.points[0].W)[0];
        test[36] = BitConverter.GetBytes(heatMapData.points[0].H)[0];
        test[40] = BitConverter.GetBytes(heatMapData.points[0].C)[0];
        test[44] = BitConverter.GetBytes(heatMapData.points[0].T)[0];

        /*UnityEngine.Debug.Log($"Test bytes length: {test.Length}, Size : {test[0]}, Id : {test[1]}, Version : {test[2]}, PlayerCount : {test[3]}, PointsCount : {test[4]}, X : {test[5]}, Y : {test[6]}, Z : {test[7]}, W : {test[8]}, H : {test[9]}, C : {test[10]}, T : {test[11]}");
        UnityEngine.Debug.Log($"Test bytes length: {test.Length}, " +
            $"Size : {BitConverter.ToInt32(test, 0)}, " +
            $"Id : {BitConverter.ToInt32(test, 4)}, " +
            $"Version : {BitConverter.ToInt32(test, 8)}, " +
            $"PlayerCount : {BitConverter.ToInt32(test, 12)}, " +
            $"PointsCount : {BitConverter.ToInt32(test, 16)}, " +
            $"X : {BitConverter.ToInt32(test, 20)}, " +
            $"Y : {BitConverter.ToInt32(test, 24)}, " +
            $"Z : {BitConverter.ToInt32(test, 28)}, " +
            $"W : {BitConverter.ToInt32(test, 32)}, " +
            $"H : {BitConverter.ToInt32(test, 36)}, " +
            $"C : {BitConverter.ToInt32(test, 40)}, " +
            $"T : {BitConverter.ToInt32(test, 44)}");
        Debug.Log(BitConverter.ToString(test));

        int test2 = -4;
        Debug.Log(BitConverter.ToString(BitConverter.GetBytes(test2)));*/
        #endregion


        //UnityEngine.Debug.Log($"Heatmap points count: {heatMapData.points.Count}, Combined bytes length: {mapInfo.Length}");

        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "heatmap.bin"), mapInfo);

        return mapInfo;
    }

    public static HeatMapData ConvertByteToMap(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 5 * sizeof(short))
        {
            Debug.LogError("Invalid byte array for HeatMapData conversion.");
            return null;
        }

        int index = 0;

        HeatMapData heatMapData = new HeatMapData
        (
            BitConverter.ToInt16(bytes, index), //0 - 1
            BitConverter.ToInt16(bytes, index + 2), //2 - 3
            BitConverter.ToInt16(bytes, index + 4), //4 - 5
            BitConverter.ToInt16(bytes, index + 6) //6 - 7
        );

        index += 8; // Move past the first 4 shorts

        int pointsCount = BitConverter.ToInt16(bytes, index); //8 - 9
        index += 2; // Move past the points count, now index is at 10

        heatMapData.points = new List<HeatPoint>();

        for (int i = 0; i < pointsCount; i++)
        {
            HeatPoint point = new HeatPoint
            (
                new List<int> { 
                    BitConverter.ToInt16(bytes, index),      //10 - 11
                    BitConverter.ToInt16(bytes, index + 2),  //12 - 13
                    BitConverter.ToInt16(bytes, index + 4)   //14 - 15
                },
                BitConverter.ToInt16(bytes, index + 6),     //16 - 17
                BitConverter.ToInt16(bytes, index + 8),     //18 - 19
                BitConverter.ToInt16(bytes, index + 10),     //20 - 21
                BitConverter.ToInt16(bytes, index + 12)      //22 - 23
            );
            heatMapData.points.Add(point);
            index += 7 * 2; // Move to the next point (7 shorts per point)
        }

        File.WriteAllText(Path.Combine(Application.persistentDataPath, "heatmapFromByte.json"), ConvertHeatMapDataToJson(heatMapData));

        return heatMapData;
    }
}
