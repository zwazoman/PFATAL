using UnityEngine;

public class ScoreboardDisplay : MonoBehaviour
{
    [SerializeField] private ScoreboardUI scoreboardPanel;
    [SerializeField] private GameObject EndGamePanel;

    void OnEnable()
    {
        GameManager.Instance.EventOnGameEnded += OnGameEnded;
        
    }

    void OnDisable()
    {
        GameManager.Instance.EventOnGameEnded -= OnGameEnded;
    }

    private void OnGameEnded(GameRulesBase.GameResult result)
    {
        EndGamePanel.SetActive(true);
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Tab))
        // {
        //     ShowScoreboard();
        // }
        // if (Input.GetKeyUp(KeyCode.Tab))
        // {
        //     ShowScoreboard();
        // }
        // if (scoreboardPanel.gameObject.activeSelf)
        // {
        //     scoreboardPanel.RefreshUI(); //voir ou mettre pour mettre à jour quand un resultat change
        //}
    }

    public void ShowScoreboard()
    {
        scoreboardPanel.gameObject.SetActive(!scoreboardPanel.gameObject.activeSelf);
    }
}
