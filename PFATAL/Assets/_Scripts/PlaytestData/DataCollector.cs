using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataCollector : MonoBehaviour
{
    private Game game;
    private List<GamePlayerSet> gamePlayerSets = new();
    private List<Score> scores = new();
    private List<Death> deaths = new();

    private int gameId = 0; 
    [SerializeField] private DatabaseRequest _databaseRequest;

    /// <summary>
    /// Fonction pour enregistrer les données d'une partie.
    /// </summary>
    public void RecordGame()
    {
        StartCoroutine(_databaseRequest.LastGameIdCoroutine((id) =>
        {
            gameId = id + 1;

            game = new Game
            {
                Version = int.Parse(Application.version.Replace(".", "")),
                IdPlayerSet = gameId,
                HeatMap = "heatmap_data",
                GameMode = (int)GameManager.gameMode,
                MapName = SceneManager.GetActiveScene().name,
                Duration = GameManager.DEATH_MATCH_GAME_DURATION
            };

            StartCoroutine(_databaseRequest.SendGame(game,(result) =>
            {
                RecordPlayerSet();
                RecordScore();
                RecordDeath();
            }));
        }));
    }

    /// <summary>
    /// Fonction pour enregistrer les données des joueurs dans une partie.
    /// </summary>
    public void RecordPlayerSet()
    {
        foreach (var entry in GameManager.Instance.LeaderBoard.entries)
        {
            gamePlayerSets.Add(new GamePlayerSet
            {
                Id = gameId,
                IdGame = gameId,
                IdPlayer = (int)entry.ClientID,
            });
        }

        StartCoroutine(_databaseRequest.SendGamePlayerSet(gamePlayerSets));
    }

    /// <summary>
    /// Fonction pour enregistrer les données de score d'une partie.
    /// </summary>
    public void RecordScore()
    {
        foreach (var entry in GameManager.Instance.LeaderBoard.entries)
        {
            scores.Add(new Score
            {
                IdGame = gameId,
                IdPlayer = (int)entry.ClientID,
                Points = entry.Points
            });
        }

        StartCoroutine(_databaseRequest.SendScore(scores));
    }

    /// <summary>
    /// Fonction pour enregistrer les données de mort d'une partie.
    /// </summary>
    public void RecordDeath()
    {
        StartCoroutine(_databaseRequest.SendDeath(deaths));
    }

    public void RegisterDeath(ulong victim, ulong killer, float time)
    {
        deaths.Add(new Death
        {
            VictimId = (int)victim,
            KillerId = (int)killer,
            Weapon = 0,
            Distance = 0f,
            IdGame = gameId,
            Time = time
        });
    }
}
