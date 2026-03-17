using UnityEngine;

public class ScoreboardDisplay : MonoBehaviour
{
    [SerializeField] private ScoreboardUI scoreboardPanel;

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
