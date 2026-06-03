using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField] public ScoreboardUI ScoreboardUIEndGamePanel;
    [SerializeField] private GameObject scoreboardEndGamePanel;

    void OnEnable()
    {
        GameManager.Instance.EventOnGameEnded += OnGameEnded;
    }

    void Start()
    {
        scoreboardEndGamePanel.SetActive(false);
        if(ScoreboardUIEndGamePanel == null) return;
        ScoreboardUIEndGamePanel.gameObject.SetActive(false);
    }

    private void OnGameEnded(GameRulesBase.GameResult result)
    {
        scoreboardEndGamePanel.SetActive(true);
        if(ScoreboardUIEndGamePanel == null) return;
        ScoreboardUIEndGamePanel.gameObject.SetActive(true);

        result.LeaderBoard.ResolvePlayerNames();
        LeaderBoardBetweenScene.Instance.SetLeaderBoardData(result);

        foreach (var item in result.LeaderBoard.entries)
        {
            Debug.Log("Adding player to end game leaderboard: " + result);
            Debug.Log("[LeaderBoardDisplay] Player Name : " + item.PlayerName);
            ScoreboardUIEndGamePanel.AddPlayerCard(item.PlayerName.ToString(), item.Points, item.Kills, item.Deaths, 99);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(ChangeScene("PodiumScene"));
    }
    
    private IEnumerator ChangeScene(string lobbyName)
    {
        float timer = 3;
        while (timer > 0f)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }
        
        var objectsToDespawn = new List<NetworkObject>(
            NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
        );

        foreach (NetworkObject netObj in objectsToDespawn)
        {
            if (netObj == null) continue;
            int layer = netObj.gameObject.layer;
            if (layer == 8 || layer == 7)
                netObj.Despawn(true);
        }

        NetworkManager.Singleton.SceneManager.LoadScene(lobbyName, LoadSceneMode.Single);
    }
}