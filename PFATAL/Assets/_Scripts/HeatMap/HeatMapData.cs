using System;
using System.Collections.Generic;

[Serializable]
public class HeatMapData
{
    public float heatMapCellSize;
    public int heatMapGameId = 0;
    public int heatMapPlayerNumber;

    public List<HeatPoint> points = new List<HeatPoint>();

    public HeatMapData(float cellSize) => (this.heatMapCellSize) = (cellSize);
}

[Serializable]
public class HeatPoint
{
    public float x, y, z;

    public int visitsGlobal;

    /*public int playerWithWeapon1Visits = 1;
    public int playerWithWeapon2Visits = 1;
    public int playerWithWeapon3Visits = 1;*/


    public HeatPoint(float x, float y, float z) => (this.x, this.y, this.z, this.visitsGlobal) = (x, y, z, 1);
    public HeatPoint(float x, float y, float z, int visitsGlobal) => (this.x, this.y, this.z, this.visitsGlobal) = (x, y, z, visitsGlobal);
}