using TMPro;
using UnityEngine;

public class PlayerCardUI : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI playerKillsText;
    public TextMeshProUGUI playerDeathsText;
    public TextMeshProUGUI playerRankText;

    public void SetPlayerInfo(string playerName,int score , int kills, int deaths , int rank)
    {
        playerNameText.text = playerName;
        playerScoreText.text = score.ToString();
        playerKillsText.text = kills.ToString();
        playerDeathsText.text = deaths.ToString();
        playerRankText.text = rank.ToString();
    }

}
