using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private Transform contentParent;

    [SerializeField] public Color colorOwner;
    [SerializeField] public Color colorDefault;

    public bool IsScoreboardEndGamePanel = false;   

    private List<PlayerCardUI> playerCards = new List<PlayerCardUI>();
    
    LeaderBoardData leaderBoardData;

    public void Awake()
    {
        if (IsScoreboardEndGamePanel) {CreateLeaderBoard(); return;}
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
            var color = player.ClientID == NetworkManager.Singleton.LocalClientId ? colorOwner : colorDefault;
            AddPlayerCard(player.ClientID.ToString(), player.Points, player.Kills, player.Deaths, player.Rank, color);
            Debug.LogWarning("Added player card for ClientID: " + player.ClientID);
        }
    }

    public void RefreshUI(ScoreEntry scoreEntry)
    {
        int i = 0;

        foreach (var player in GameManager.Instance.LeaderBoard.entries)
        {
            PlayerCardUI card = playerCards[i];
            var color = player.ClientID == NetworkManager.Singleton.LocalClientId ? colorOwner : colorDefault;
            card.SetPlayerInfo(player.PlayerName.ToString(), player.Points, player.Kills, player.Deaths, player.Rank, color);

            card.transform.SetSiblingIndex(i);
            i++;
        }
    }

    public void AddPlayerCard(string playerName, int score, int kills, int deaths, int rank, Color color)
    {
        GameObject newCard = Instantiate(playerCardPrefab, contentParent);
        PlayerCardUI cardUI = newCard.GetComponent<PlayerCardUI>();
        cardUI.SetPlayerInfo(playerName, score, kills, deaths, rank, color);
        playerCards.Add(cardUI);
    }

    public void CreateLeaderBoard()
    {
        foreach (var card in playerCards)
            Destroy(card.gameObject);
        playerCards.Clear();
        
        leaderBoardData = LeaderBoardDataBetweenScene.Instance.GetLeaderBoardData();
        Debug.Log(leaderBoardData.ToString());
        
        foreach (var player in leaderBoardData.entries)
        {
            var color = player.ClientID == NetworkManager.Singleton.LocalClientId ? colorOwner : colorDefault;

            AddPlayerCard(
                player.PlayerName.ToString(),
                player.Points,
                player.Kills,
                player.Deaths,
                player.Rank,
                color
            );
        }
    }

}