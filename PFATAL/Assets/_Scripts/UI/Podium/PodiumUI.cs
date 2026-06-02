using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PodiumUI : MonoBehaviour
{
    public Transform playerCharacterParent;
    public Transform namePlayerParent;
    
    public List<string> leaderBoardData;

    private void Awake()
    {
        foreach (Transform child in playerCharacterParent.transform)
        {
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in namePlayerParent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        for (int i = 0; i < leaderBoardData.Count; i++)
        {
            if (i >= 7) return;
            playerCharacterParent.GetChild(i).gameObject.SetActive(true);
            namePlayerParent.GetChild(i).gameObject.SetActive(true);
            namePlayerParent.GetChild(i).gameObject.GetComponent<GametagUI>().SetPlayerName(leaderBoardData[i]);
        }
    }
}
