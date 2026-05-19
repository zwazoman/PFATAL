using UnityEngine;

[CreateAssetMenu(fileName = "new Game Settings", menuName = "Game Settings")]
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