using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField] public ScoreboardUI ScoreboardUIEndGamePanel;

    void OnEnable()
    {
        GameManager.Instance.EventOnGameEnded += OnGameEnded;
    }

    void OnDisable()
    {
        GameManager.Instance.EventOnGameEnded -= OnGameEnded;
    }

    private void OnGameEnded(GameRulesBase.GameResult result)
    {
        ScoreboardUIEndGamePanel.gameObject.SetActive(true);
        foreach (var item in result.LeaderBoard.entries)
        {
            ScoreboardUIEndGamePanel.AddPlayerCard(item.ClientID.ToString(), item.Points, item.Kills, item.Deaths, 99);
        }
    }
}