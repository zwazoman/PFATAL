using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderBoardBetweenScene : MonoBehaviour
{
    public static LeaderBoardBetweenScene Instance { get; private set; }
    
    LeaderBoardData _leaderBoardData;
    
    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void SetLeaderBoardData(GameRulesBase.GameResult result)
    {
        if (_leaderBoardData == null) _leaderBoardData = new LeaderBoardData();

        _leaderBoardData.entries = result.LeaderBoard.entries;
    }

    public LeaderBoardData GetLeaderBoardData()
    {
        return _leaderBoardData;
    }
}
