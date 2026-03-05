using System;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private LeaderBoardData leaderBoardData;

    void Start()
    {
        // Simulate adding player cards to the scoreboard
        for (int i = 0; i < 4; i++)
        {
            AddPlayerCard($"Player {i + 1}", 0, 0, UnityEngine.Random.Range(10, 100));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshUI()
    {
        
    }

    public void AddPlayerCard(string playerName = "Player", int kills = 0, int deaths = 0, int ping = 0)
    {
        GameObject newCard = Instantiate(playerCardPrefab, contentParent);
        PlayerCardUI cardUI = newCard.GetComponent<PlayerCardUI>();
        cardUI.SetPlayerInfo(playerName, kills, deaths, ping);
    }
}
