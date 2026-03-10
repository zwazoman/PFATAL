using NaughtyAttributes;
using System.IO;
using UnityEngine;

public class CollectData : MonoBehaviour
{
    private string path;

    [SerializeField] private DriveGetter _driveGetter;
    [SerializeField] private DriveSender _driveSender;

    public StatData statData = new StatData();
    public SessionData currentSession = new SessionData();

    void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    [Button]
    public void AddPlayerStat()
    {

        PlayerStatData playerStatData = new PlayerStatData
        {
            Id = 0,
            GameId = 0,
            PlayerName = "Player_01",
            Kills = 7,
            Deaths = 4,
            MainWeapon = "CrossBow",
            TimePercentageUsed = 42.7f,
            Accuracy = 80.8f,
            ConsomableUsed = 13,
            GameVersion = Application.version
        };

        currentSession.Stats.Add(playerStatData);
    }

    [Button]
    public void SaveSession()
    {
        statData.Sessions.Add(currentSession);

        string newJson = JsonUtility.ToJson(statData, true);
        File.WriteAllText(path, newJson);

        _driveSender.SendData(File.ReadAllText(path));
    }

    [Button]
    public void LoadData()
    {
        if (!File.Exists(_driveGetter.JsonData)) return;

        string json = File.ReadAllText(_driveGetter.JsonData);
        statData = JsonUtility.FromJson<StatData>(json);
    }
}