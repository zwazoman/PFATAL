using System;
using System.Collections.Generic;

[Serializable]
public class PlayerStatData
{
    public int Id;
    public int GameId;
    public string PlayerName;
    public int Kills;
    public int Deaths;
    public string MainWeapon;
    public float TimePercentageUsed;
    public float Accuracy;
    public int ConsomableUsed;
    public string GameVersion;
}

[Serializable]
public class SessionData
{
    public List<PlayerStatData> Stats = new List<PlayerStatData>();
}

[Serializable]
public class StatData
{
    public List<SessionData> Sessions = new List<SessionData>();
}