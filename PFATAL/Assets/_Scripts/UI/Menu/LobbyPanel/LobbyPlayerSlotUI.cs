using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LobbyPlayerSlotUI : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] TMP_Text _playerNameText;
    [SerializeField] TMP_Text _statusText;
    [SerializeField] int oui;
    [SerializeField] Image _skinColorImage;

    [SerializeField] Color[] _skinColors;

    public void SetData(LobbyPlayerData data)
    {
        _playerNameText.text = data.DisplayName;
        _skinColorImage.color = _skinColors[data.skinID];
        _statusText.text = data.status.ToString();
    }
}
