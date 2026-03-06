using System.Collections.Generic;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private Transform contentParent;

    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject scoreboardPanel;

    private List<PlayerCardUI> playerCards = new List<PlayerCardUI>();

    void OnEnable()
    {
        gameManager.EventOnGameEnded += OnGameEnded;
    }

    void OnDisable()
    {
        gameManager.EventOnGameEnded -= OnGameEnded;
    }

    private void OnGameEnded(GameRulesBase.GameResult result)
    {
        scoreboardPanel.SetActive(true);
    }


    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowScoreboard();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ShowScoreboard();
        }
        if (scoreboardPanel.activeSelf)
        {
            RefreshUI(); //voir ou mettre pour mettre à jour quand un resultat change
        }
    }

    public void RefreshUI()
    {
        int i = 0;
        foreach (var player in gameManager.LeaderBoard.entries)
        {
            PlayerCardUI card = playerCards[i];

            card.SetPlayerInfo(player.ClientID.ToString(), player.Points, player.Kills, player.Deaths, 99);
            card.transform.SetSiblingIndex(i);
            i++;
        }
    }

    public void AddPlayerCard(string playerName = "Player", int kills = 0, int deaths = 0, int ping = 0)
    {
        GameObject newCard = Instantiate(playerCardPrefab, contentParent);
        PlayerCardUI cardUI = newCard.GetComponent<PlayerCardUI>();
        cardUI.SetPlayerInfo(playerName, kills, deaths, ping);
        playerCards.Add(cardUI);
    }

    public void ShowScoreboard()
    {
        scoreboardPanel.SetActive(!scoreboardPanel.activeSelf);
    }
}