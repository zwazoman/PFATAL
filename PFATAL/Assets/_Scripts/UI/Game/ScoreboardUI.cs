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
            AddPlayerCard(player.ClientID.ToString(), player.Points, player.Kills, player.Deaths, player.Rank);
            Debug.LogWarning("Added player card for ClientID: " + player.ClientID);
        }
    }

    public void RefreshUI()
    {
        int i = 0;

        foreach (var player in GameManager.Instance.LeaderBoard.entries)
        {
            PlayerCardUI card = playerCards[i];
            card.SetPlayerInfo(player.ClientID.ToString(), player.Points, player.Kills, player.Deaths,player.Rank);
            card.transform.SetSiblingIndex(i);
            i++;
        }
    }

    public void AddPlayerCard(string playerName, int score, int kills, int deaths, int rank)
    {
        GameObject newCard = Instantiate(playerCardPrefab, contentParent);
        PlayerCardUI cardUI = newCard.GetComponent<PlayerCardUI>();
        cardUI.SetPlayerInfo(playerName, score, kills, deaths, rank);
        playerCards.Add(cardUI);
    }

}