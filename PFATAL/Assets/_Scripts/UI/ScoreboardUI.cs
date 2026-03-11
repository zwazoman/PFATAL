using System.Collections.Generic;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private Transform contentParent;

    public bool IsScoreboardEndGamePanel = false;   

    private List<PlayerCardUI> playerCards = new List<PlayerCardUI>();

    public void Awake()
    {
        if (IsScoreboardEndGamePanel) return;
        GameManager.Instance.EventOnGameStarted += InitializeScoreboard;
        Debug.LogWarning("ScoreboardUI subscribed to GameManager's EventOnGameStarted.");

        gameObject.SetActive(false);
    }


    public void InitializeScoreboard()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager reference is missing in ScoreboardUI.");
            return;
        }
        if (GameManager.Instance.LeaderBoard.entries == null)
        {
            Debug.LogError("GameManager's LeaderBoard is not initialized.");
            return;
        }

        foreach (var player in GameManager.Instance.LeaderBoard.entries)
        {
            AddPlayerCard(player.ClientID.ToString(), player.Points, player.Kills, player.Deaths, 99);
            Debug.LogWarning("Added player card for ClientID: " + player.ClientID);
        }
    }

    public void RefreshUI(GameRulesBase.GameResult result)
    {
        int i = 0;

        foreach (var player in GameManager.Instance.LeaderBoard.entries)
        {
            PlayerCardUI card = playerCards[i];
            card.SetPlayerInfo(player.ClientID.ToString(), player.Points, player.Kills, player.Deaths, 99);
            card.transform.SetSiblingIndex(i);
            i++;
        }
    }

    public void AddPlayerCard(string playerName = "Player", int score = 0, int kills = 0, int deaths = 0, int ping = 0)
    {
        GameObject newCard = Instantiate(playerCardPrefab, contentParent);
        PlayerCardUI cardUI = newCard.GetComponent<PlayerCardUI>();
        cardUI.SetPlayerInfo(playerName, score, kills, deaths, ping);
        playerCards.Add(cardUI);
    }

}