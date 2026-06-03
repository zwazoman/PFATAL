using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PodiumUI : MonoBehaviour
{
    public Transform playerCharacterParent;
    public Transform namePlayerParent;
    
    public LeaderBoardData leaderBoardData;

    private void Awake()
    {
        leaderBoardData = LeaderBoardDataBetweenScene.Instance.GetLeaderBoardData();
        Debug.Log("PodiumUI : " +  leaderBoardData.entries);
        foreach (Transform child in playerCharacterParent.transform)
        {
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in namePlayerParent.transform)
        {
            child.gameObject.SetActive(false);
            child.GetComponent<GametagUI>().SetPlayerName("");
        }
    }

    public void Start()
    {
        NamePlayerDisplay();
    }

    private async void NamePlayerDisplay()
    {
        await Task.Delay(100);

        int i = 0;

        foreach (var entry in leaderBoardData.entries)
        {
            playerCharacterParent.GetChild(i).gameObject.SetActive(true);

            var gametag = namePlayerParent.GetChild(i).GetComponent<GametagUI>();
            gametag.gameObject.SetActive(true);
            gametag.SetPlayerName(entry.PlayerName.ToString());
            i++;
        }
    }
}
