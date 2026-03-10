using Unity.VisualScripting.Dependencies.Sqlite;

public class PlaySession
{
    [NotNull, PrimaryKey, AutoIncrement, Unique]
    public int Id { get; set; }
    public int GameId { get; set; }
    public string PlayerName { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public string MainWeapon { get; set; }
    public float Accuracy { get; set; }
    public string GameVersion { get; set; }
}

public class PositionLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int SessionId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float TimeStamp { get; set; }
    public int Visit { get; set; }
    public string GameVersion { get; set; }
}