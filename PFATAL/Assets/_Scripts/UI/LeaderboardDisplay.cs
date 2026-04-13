using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField] public ScoreboardUI ScoreboardUIEndGamePanel;
    [SerializeField] private GameObject scoreboardEndGamePanel;

    void OnEnable()
    {
        GameManager.Instance.EventOnGameEnded += OnGameEnded;
    }

    void OnDisable()
    {
        GameManager.Instance.EventOnGameEnded -= OnGameEnded;
    }

    void Start()
    {
        scoreboardEndGamePanel.SetActive(false);
        ScoreboardUIEndGamePanel.gameObject.SetActive(false);
    }

    private void OnGameEnded(GameRulesBase.GameResult result)
    {
        scoreboardEndGamePanel.SetActive(true);
        ScoreboardUIEndGamePanel.gameObject.SetActive(true);

        Debug.Log("[LeaderboardDisplay] OnGameEnded appelé");
        Debug.Log("[LeaderboardDisplay] LeaderBoard null : " + (result.LeaderBoard == null));
        Debug.Log("[LeaderboardDisplay] Nombre d'entrées : " + result.LeaderBoard.entries.Count);

        result.LeaderBoard.ResolvePlayerNames();

        foreach (var item in result.LeaderBoard.entries)
        {
            Debug.Log("Adding player to end game leaderboard: " + result);
            Debug.Log("[LeaderBoardDisplay] Player Name : " + item.PlayerName);
            ScoreboardUIEndGamePanel.AddPlayerCard(item.PlayerName.ToString(), item.Points, item.Kills, item.Deaths, 99);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}