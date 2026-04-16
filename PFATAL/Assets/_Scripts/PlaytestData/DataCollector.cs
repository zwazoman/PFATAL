using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataCollector : MonoBehaviour
{
    public static DataCollector Instance;
    private Game game;
    private List<GamePlayerSet> gamePlayerSets = new();
    private List<Score> scores = new();
    private List<Death> deaths = new();
    private int gameId = 0;
    private string apiBaseUrl = "http://localhost:5000";
    [SerializeField] private DatabaseRequest _databaseRequest;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        GameManager.Instance.EventOnGameStarted += RecordPlayers;
        GameManager.Instance.EventOnGameEnded += OnGameEnded;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.EventOnGameStarted -= RecordPlayers;

        if (GameManager.Instance != null) GameManager.Instance.EventOnGameEnded -= OnGameEnded;
    }

    private void OnGameEnded(GameRulesBase.GameResult result)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        RecordGame(result.LeaderBoard);
    }

    private IEnumerator PlayerExistsCoroutine(int playerId, System.Action<bool> callback)
    {
        string url = $"{apiBaseUrl}/player/exists/{playerId}";
        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[DataCollector] Impossible de vérifier le joueur {playerId} : {request.error}. Remplacement par 0.");
            callback(false);
            yield break;
        }

        bool exists = request.downloadHandler.text.Contains("\"exists\":true");
        callback(exists);
    }

    private IEnumerator ValidatePlayerIds(List<int> playerIds, System.Action<Dictionary<int, int>> callback)
    {
        Dictionary<int, int> validatedIds = new();

        foreach (int id in playerIds)
        {
            // Si déjà vérifié, on skip
            if (validatedIds.ContainsKey(id)) continue;

            bool exists = false;
            yield return StartCoroutine(PlayerExistsCoroutine(id, result => exists = result));
            validatedIds[id] = exists ? id : 0;

            if (!exists)
                Debug.LogWarning($"[DataCollector] Joueur {id} déjà dans le DB, retourne 0.");
        }

        callback(validatedIds);
    }

    public void RecordPlayers()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Player player = new Player
            {
                Id = (long)clientID,
                Name = "Player_" + clientID
            };

            StartCoroutine(_databaseRequest.SendPlayer(player));
        }
    }

    /// <summary>
    /// Fonction pour enregistrer les données d'une partie.
    /// </summary>
    public void RecordGame(LeaderBoardData leaderBoard)
    {
        gamePlayerSets.Clear();
        scores.Clear();

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

            StartCoroutine(_databaseRequest.SendGame(game, (result) =>
            {
                RecordPlayerSet(leaderBoard);
                RecordScore(leaderBoard);
                RecordDeath();
            }));
        }));
    }

    /// <summary>
    /// Fonction pour enregistrer les données des joueurs dans une partie.
    /// </summary>
    public void RecordPlayerSet(LeaderBoardData leaderBoard)
    {
        foreach (var entry in leaderBoard.entries)
        {
            gamePlayerSets.Add(new GamePlayerSet
            {
                Id = gameId,
                IdGame = gameId,
                IdPlayer = (long)entry.ClientID,
            });
        }

        StartCoroutine(_databaseRequest.SendGamePlayerSet(gamePlayerSets));
    }

    /// <summary>
    /// Fonction pour enregistrer les données de score d'une partie.
    /// </summary>
    public void RecordScore(LeaderBoardData leaderBoard)
    {
        foreach (var entry in leaderBoard.entries)
        {
            scores.Add(new Score
            {
                IdGame = gameId,
                IdPlayer = (long)entry.ClientID,
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

    public void RegisterDeath(ulong victim, ulong killer, float distance, float time)
    {
        deaths.Add(new Death
        {
            VictimId = (long)victim,
            KillerId = (long)killer,
            Weapon = 0,
            Distance = distance,
            IdGame = gameId,
            Time = time
        });
    }
}