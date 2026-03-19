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

    private void GetLastGameId()
    {
        StartCoroutine(_databaseRequest.LastGameIdCoroutine((id) =>
        {
            gameId = id + 1;
            Debug.Log("ID récupéré : " + id);
        }));
    }

    public void RecordGame()
    {
        GetLastGameId();

        game = new Game
        {
            Version = int.Parse(Application.version.Replace(".", "")),
            IdPlayerSet = gameId,
            HeatMap = "heatmap_data",
            //GameMode = GameManager.GameMode.DeathMatch.ToString,
            MapName = SceneManager.GetActiveScene().name,
            Duration = 300f
        };
    }

    public void RecordPlayerSet()
    {
        for (int i = 0; i < 4; i++)
        {
            gamePlayerSets.Add(new GamePlayerSet
            {
                Id = gameId,
                IdGame = gameId,
                IdPlayer = 1,
            });
        }
    }

    public void RecordScore()
    {
        for (int i = 0; i < 4; i++)
        {
            scores.Add(new Score
            {
                IdGame = gameId,
                IdPlayer = 1,
                Points = 1
            });
        }
    }

    public void RecordDeath()
    {
        for (int i = 0; i < 4; i++)
        {
            deaths.Add(new Death
            {
                VictimId = 1,
                KillerId = 1,
                Weapon = 1,
                Distance = 10f,
                IdGame = gameId,
                Time = 60f
            });
        }
    }

}
