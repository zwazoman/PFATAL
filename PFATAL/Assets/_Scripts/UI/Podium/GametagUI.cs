using TMPro;
using UnityEngine;

public class GametagUI : MonoBehaviour
{
    public TextMeshProUGUI namePlayerText;
    
    public void SetPlayerName(string playerName)
    {
        namePlayerText.text = playerName;
    }
}
