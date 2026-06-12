using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PodiumUI : MonoBehaviour
{
    [Header("Podium")]
    public Transform playerCharacterParent;
    public Transform namePlayerParent;

    [Header("Caméra")]
    [SerializeField] private CameraSetupOwner cameraSetupOwner;

    private LeaderBoardData leaderBoardData;
    private Dictionary<ulong, int> _pendingSkins = new();

    private void Awake()
    {
        leaderBoardData = LeaderBoardDataBetweenScene.Instance.GetLeaderBoardData();

        foreach (Transform child in playerCharacterParent)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in namePlayerParent)
        {
            child.gameObject.SetActive(false);
            child.GetComponent<GametagUI>().SetPlayerName("");
        }
    }

    private void Start()
    {
        NamePlayerDisplay();
    }

    private async void NamePlayerDisplay()
    {
        await Task.Delay(100);
        int i = 0;
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        Transform localCharacter = null;

        foreach (var entry in leaderBoardData.entries)
        {
            try
            {
                Transform character = playerCharacterParent.GetChild(i);
                character.gameObject.SetActive(true);

                if (_pendingSkins.TryGetValue(entry.ClientID, out int skinID))
                {
                    Debug.Log($"[PodiumUI] i={i} client={entry.ClientID} skinID={skinID}");
                    character.GetComponent<SkinHandler>()?.SwapSkin(skinID);
                }

                if (entry.ClientID == localClientId)
                    localCharacter = character;

                var gametag = namePlayerParent.GetChild(i).GetComponent<GametagUI>();
                gametag.gameObject.SetActive(true);
                gametag.SetPlayerName(entry.PlayerName.ToString());
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PodiumUI] Erreur à i={i} : {e}");
            }

            i++;
        }

        if (localCharacter != null)
            cameraSetupOwner.SetTarget(localCharacter, playerCharacterParent, namePlayerParent);
    }

    public void ApplySkin(ulong clientId, int skinID)
    {
        _pendingSkins[clientId] = skinID;

        int i = 0;
        foreach (var entry in leaderBoardData.entries)
        {
            if (entry.ClientID == clientId)
            {
                var character = playerCharacterParent.GetChild(i);
                if (character.gameObject.activeSelf)
                    character.GetComponent<SkinHandler>().SwapSkin(skinID);
                return;
            }
            i++;
        }
    }
}