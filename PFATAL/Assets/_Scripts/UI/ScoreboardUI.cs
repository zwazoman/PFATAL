using System.Collections.Generic;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private Transform contentParent;

    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject scoreboardPanel;

    private List<PlayerCardUI> playerCards = new List<PlayerCardUI>();

    void LateUpdate()
    {
        if (scoreboardPanel.activeSelf)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        foreach (var player in gameManager.LeaderBoard.entries)
        {
            for (int i = 0; i < playerCards.Count; i++)
            {   
                if (player.ClientID.ToString() == playerCards[i].playerNameText.text)
                {
                    playerCards[i].UpdatePlayerInfo(player.Kills, player.Deaths, UnityEngine.Random.Range(10, 100));
                }
            }
        }
    }

    public void AddPlayerCard(string playerName = "Player", int kills = 0, int deaths = 0, int ping = 0)
    {
        GameObject newCard = Instantiate(playerCardPrefab, contentParent);
        PlayerCardUI cardUI = newCard.GetComponent<PlayerCardUI>();
        cardUI.SetPlayerInfo(playerName, kills, deaths, ping);
        playerCards.Add(cardUI);
    }


    // Change la visibilité du Scoreboard lorsque le joueur appuie sur la touche Tab (ptet faire que quand le joueur maintient la touche Tab)
    public void ShowScoreboard()
    {
        scoreboardPanel.SetActive(!scoreboardPanel.activeSelf);
    }
}
