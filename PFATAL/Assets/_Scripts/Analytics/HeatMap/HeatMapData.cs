using System;
using System.Collections.Generic;

public enum HeatMapType
{
    Player = 0,
    Game = 1
}

/// <summary>
/// Represents the configuration and data for a heat map, including cell size, game information, player number, heat map
/// type, and a collection of heat points.
/// </summary>
/// <remarks>Use this class to store and transfer heat map data for analysis or visualization purposes. The
/// properties provide context about the game session and player, while the points collection holds individual heat map
/// data points. This type is serializable for persistence or network transmission.</remarks>
[Serializable]
public class HeatMapData
{
    public int heatMapCellSize;
    public int heatMapGameId;
    public string heatMapGameVersion;
    public int heatMapPlayerNumber;
    public HeatMapType heatMapType;

    public List<HeatPoint> points = new List<HeatPoint>();

    /// <summary>
    /// Create an empty heatMapData object with a defined size
    /// </summary>
    /// <param name="cellSize">Size of the heatmap</param>
    public HeatMapData(int cellSize) => (this.heatMapCellSize) = (cellSize);

    /// <summary>
    /// Create an empty heatMapData object with some basics informations about the heatMap
    /// </summary>
    /// <param name="cellSize">Size of the heatmap</param>
    /// <param name="heatMapGameId">Game id of the game that created the heatmap</param>
    /// <param name="heatMapGameVersion">Game version when the heatmap was created</param>
    /// <param name="heatMapPlayerNumber">Number of players that particpate to make the heatmap</param>
    public HeatMapData(int cellSize, int heatMapGameId, string heatMapGameVersion, int heatMapPlayerNumber) => 
        (this.heatMapCellSize, this.heatMapGameId, this.heatMapGameVersion, this.heatMapPlayerNumber) = (cellSize, heatMapGameId, heatMapGameVersion, heatMapPlayerNumber);
}

/// <summary>
/// Points that compose the heatmap, containing coordinate and data about which type of player walk through
/// </summary>
[Serializable]
public class HeatPoint
{
    /// <summary>
    /// point coordinates
    /// </summary>
    public float x, y, z;

    /// <summary>
    /// point total visit number
    /// </summary>
    public int visitsGlobal;

    /// <summary>
    /// Visits of player with hammer
    /// </summary>
    public int playerWithHammerVisits;

    /// <summary>
    /// Visits of player with crossbow
    /// </summary>
    public int playerWithCrossbowVisits;

    /// <summary>
    /// Visits of player with tomahawk
    /// </summary>
    public int playerWithTomahawkVisits;

    /// <summary>
    /// Create a new point with x, y and z coordinates, initialize the total visits number to 1
    /// </summary>
    /// <param name="x">X coordinate of the point</param>
    /// <param name="y">Y coordinate of the point</param>
    /// <param name="z">Z coordinate of the point</param>
    public HeatPoint(float x, float y, float z) => (this.x, this.y, this.z, this.visitsGlobal) = (x, y, z, 1);

    /// <summary>
    /// Create a new points with x, y and z cordinates, and informations about players that visits it
    /// </summary>
    /// <param name="x">X coordinate of the point</param>
    /// <param name="y">Y coordinate of the point</param>
    /// <param name="z">Z coordinate of the point</param>
    /// <param name="visitsGlobal">Total visits of the point</param>
    /// <param name="hammerVisit">Visits of player with hammer</param>
    /// <param name="crossbowVisit">Visits of player with crossbow</param>
    /// <param name="tomahawkVisits">Visits of player with tomahawk</param>
    public HeatPoint(float x, float y, float z, int visitsGlobal, int hammerVisit, int crossbowVisit, int tomahawkVisits) => 
        (this.x, this.y, this.z, this.visitsGlobal, this.playerWithHammerVisits, this.playerWithCrossbowVisits, this.playerWithTomahawkVisits) = 
        (x, y, z, visitsGlobal, hammerVisit, crossbowVisit, tomahawkVisits);
}