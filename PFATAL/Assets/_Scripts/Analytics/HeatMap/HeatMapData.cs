using System;
using System.Collections.Generic;

public enum HeatMapType
{
    Game = 0,
    multipleGame = 1,
}

public enum WeaponType
{
    All = 0,
    Without = 1,
    Hammer = 2,
    Crossbow = 3,
    Tomahawk = 4
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
    public int cellSize;
    public int gameId;
    public string gameVersion;
    public int playerCount;
    public HeatMapType type;

    public List<HeatPoint> points = new List<HeatPoint>();

    /// <summary>
    /// Create an empty heatMapData object with a defined size, for test mostly
    /// </summary>
    /// <param name="cellSize">Size of the heatmap</param>
    public HeatMapData(int cellSize) => (this.cellSize) = (cellSize);

    /// <summary>
    /// Create an empty heatMapData object with some basics informations about the heatMap
    /// </summary>
    /// <param name="cellSize">Size of the heatmap</param>
    /// <param name="heatMapGameId">Game id of the game that created the heatmap</param>
    /// <param name="heatMapGameVersion">Game version when the heatmap was created</param>
    /// <param name="heatMapPlayerNumber">Number of players that particpate to make the heatmap</param>
    public HeatMapData(int cellSize, int heatMapGameId, string heatMapGameVersion, int heatMapPlayerNumber, HeatMapType mapType) => 
        (this.cellSize, this.gameId, this.gameVersion, this.playerCount, this.type) = 
        (cellSize, heatMapGameId, heatMapGameVersion, heatMapPlayerNumber, mapType);
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
    public List<int> P = new();

    /// <summary>
    /// point total visit number
    /// </summary>
    //public int G;

    /// <summary>
    /// Vists of player without weapons
    /// </summary>
    public int W;

    /// <summary>
    /// Visits of player with hammer
    /// </summary>
    public int H;

    /// <summary>
    /// Visits of player with crossbow
    /// </summary>
    public int C;

    /// <summary>
    /// Visits of player with tomahawk
    /// </summary>
    public int T;

    /// <summary>
    /// Create a new point with x, y and z coordinates, initialize the total visits number to 1
    /// </summary>
    /// <param name="p">Coordinate of the point</param>
    public HeatPoint(List<int> p) => (this.P/*, this.G*/) = (p/*, 1*/);

    /// <summary>
    /// Create a new points with x, y and z cordinates, and informations about players that visits it
    /// </summary>
    /// <param name="P">Coordinates of the point</param>
    /// <param name="visitsGlobal">Total visits of the point</param>
    /// <param name="hammerVisit">Visits of player with hammer</param>
    /// <param name="crossbowVisit">Visits of player with crossbow</param>
    /// <param name="tomahawkVisits">Visits of player with tomahawk</param>
    public HeatPoint(List<int> P/*, int visitsGlobal*/, int noWeaponsVisits, int hammerVisit, int crossbowVisit, int tomahawkVisits) => 
        (this.P/*, this.G*/, this.W, this.H, this.C, this.T) = 
        (P/*, visitsGlobal*/, noWeaponsVisits, hammerVisit, crossbowVisit, tomahawkVisits);

    public int GetGlobalVisits()
    {
        return W + H + C + T;
    }
}