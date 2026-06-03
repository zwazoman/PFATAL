using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderBoardDataBetweenScene : MonoBehaviour
{
    public static LeaderBoardDataBetweenScene Instance { get; private set; }
    
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
        Debug.Log(_leaderBoardData.entries);
        return _leaderBoardData;
    }

}
