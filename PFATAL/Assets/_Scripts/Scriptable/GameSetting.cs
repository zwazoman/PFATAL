using UnityEngine;

[CreateAssetMenu(fileName = "new GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSetting : ScriptableObject
{
    public float GameDuration = 60*4;
    public GameMode GameMode;
}

//data
public enum GameMode
{
    DeathMatch,
    None
}