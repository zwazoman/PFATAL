using TMPro;
using UnityEngine;


public class LobbyPlayerSlotUI : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] TMP_Text _playerNameText;
    [SerializeField] TMP_Text _statusText;

    public void SetData(LobbyPlayerData data)
    {
        _playerNameText.text = data.name;
        _statusText.text = data.status.ToString();
    }
}
