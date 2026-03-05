using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class LobbyPanelUI : MonoBehaviour
{
    [Header("scene references")]
    [SerializeField] private GameLobby gameLobby;
    [SerializeField] private Transform _playerSlotsParent;
    [SerializeField] private Image _readyImage;
    [SerializeField] private TMP_Text _statusText;
    //[SerializeField] private TMP_Text _GameModeText;
    //[SerializeField] private TMP_Text _mapNameText;
    //[SerializeField] private Image _mapImage;
    
    [Header("asset references")]
    [SerializeField] private LobbyPlayerSlotUI _playerSlotPrefab;
    [SerializeField] private GameObject _emptyPlayerSlotPrefab;

    void Awake()
    {
        gameLobby.EventOnLobbyUpdated += RefreshPlayerList;
        gameLobby.EventOnPlayerStatusChanged += OnPlayerStatusChanged;
    }
    
    public void RefreshPlayerList(PlayerList newPlayerList)
    {
        //clear existing player slots
        foreach (Transform child in _playerSlotsParent)
        {
            Destroy(child.gameObject);
        }
        
        //instantiate new slots
        for (int i = 0; i < 4; i++)
        {
            ulong[] keys = newPlayerList.dictionnary.Keys.ToArray();
            if (i < keys.Length)
            {
                LobbyPlayerSlotUI slot = Instantiate(_playerSlotPrefab, _playerSlotsParent);
                slot.SetData(newPlayerList.dictionnary[keys[i]]);
            }
            else
            {
                Instantiate(_emptyPlayerSlotPrefab, _playerSlotsParent);
            }
        }
    }

    public void OnPlayerStatusChanged(PlayerStatus status)
    {
        _readyImage.color = status switch
        {
            PlayerStatus.InGame => Color.yellow * .25f,
            PlayerStatus.Ready => Color.green * .25f,
            PlayerStatus.Waiting => Color.red * .25f,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        }; 
        _statusText.text = status.ToString();
    }

    // public void OnGameModeChanged()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public void OnMapChanged()
    // {
    //     throw new NotImplementedException();
    // }
}
