using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardUI : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerKillsText;
    public TextMeshProUGUI playerDeathsText;
    public TextMeshProUGUI playerPingText;

    public void SetPlayerInfo(string playerName = "Player", int kills = 0, int deaths = 0, int ping = 0)
    {
        playerNameText.text = playerName;
        playerKillsText.text = kills.ToString();
        playerDeathsText.text = deaths.ToString();
        playerPingText.text = ping.ToString() + " ms";
    }

    public void UpdatePlayerInfo(int kills, int deaths, int ping)
    {
        playerKillsText.text = kills.ToString();
        playerDeathsText.text = deaths.ToString();
        playerPingText.text = ping.ToString() + " ms";
    }

    public void UpdatekillsInfo(int kills)
    {
        playerKillsText.text = kills.ToString();
    }

    public void UpdateDeathsInfo(int deaths)
    {
        playerDeathsText.text = deaths.ToString();
    }

    public void UpdatePingInfo(int ping)
    {
        playerPingText.text = ping.ToString() + " ms";
    }

}
