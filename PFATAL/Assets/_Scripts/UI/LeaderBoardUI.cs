using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardUI : MonoBehaviour
{
    [SerializeField] private GameObject playerCardPrefab;
    
    [SerializeField] private Transform contentParent;

    [SerializeField] private List<GameObject> Ranks;

    [SerializeField] private GameManager gameManager;

    private List<PlayerCardUI> playerCards = new List<PlayerCardUI>();

    public class TempPlayerData
    {
        public string ClientID;
        public int Kills;
        public int Deaths;
        public int Points;
        public int Rank;
    }


    void Start()
    {
        // List<object> playerDataList = new List<object>();
        // foreach (var player in gameManager.LeaderBoard.entries)
        // {
        //     playerDataList.Add(new
        //     {
        //         player.ClientID,
        //         player.Kills,
        //         player.Deaths,
        //         player.Points,
        //         player.Rank
        //     });
        // }

        var playerDataList = new List<object>();

        TempPlayerData Player1 = new TempPlayerData { ClientID = "Player1", Kills = 5, Deaths = 2, Points = 3, Rank = 1};
        TempPlayerData Player2 = new TempPlayerData { ClientID = "Player2", Kills = 3, Deaths = 1, Points = 2, Rank = 2};
        TempPlayerData Player3 = new TempPlayerData { ClientID = "Player3", Kills = 1, Deaths = 4, Points = -3, Rank = 3};
        TempPlayerData Player4 = new TempPlayerData { ClientID = "Player4", Kills = 0, Deaths = 3, Points = -3, Rank = 4};

        playerDataList.Add(Player2);
        playerDataList.Add(Player4);
        playerDataList.Add(Player3);
        playerDataList.Add(Player1);
        

        playerDataList.Sort((a, b) => ((TempPlayerData)a).Rank - ((TempPlayerData)b).Rank);
        
        for (int i = 0; i < playerDataList.Count; i++)
        {
            TempPlayerData playerData = (TempPlayerData)playerDataList[i];
            AddPlayerCard(playerData.ClientID, playerData.Kills, playerData.Deaths, playerData.Points);
            Ranks[i].SetActive(true);
        }
    }

    public void AddPlayerCard(string playerName = "Player", int kills = 0, int deaths = 0, int score = 0)
    {
        GameObject newCard = Instantiate(playerCardPrefab, contentParent);
        PlayerCardUI cardUI = newCard.GetComponent<PlayerCardUI>();
        cardUI.ShowScoreInfo(playerName, kills, deaths, score);
        playerCards.Add(cardUI);
    }
}
