using System;
using UnityEngine;

public class CrosshairsManager : MonoBehaviour
{
    public event Action OnActivateCrosshair;

    [Header("References")]
    [SerializeField] public HUDManager hud;
    [SerializeField] GameObject _baseCrosshair;
    [SerializeField] TomahawkCrosshair _tomahawkCrosshair;
    [SerializeField] HammerCrosshair _hammerCrosshair;
    [SerializeField] CrossbowCrosshair _crossbowCrosshair;


    private void Start()
    {
        Hand hand = hud.playerCharacter.playerHands.rightHand;

        hand.OnEquipCrossbow += OnEquipCrossbow_Callback;
        hand.OnEquipHammer += OnEquipHammer_Callback;
        hand.OnEquipTomahawk += OnEquipTomahawk_Callback;

        OnActivateCrosshair += DisableBaseCrosshair;
    }

    void OnEquipTomahawk_Callback(Tomahawk tomahawk)
    {
        OnActivateCrosshair?.Invoke();
        _tomahawkCrosshair.Activate(tomahawk);
    }
    void OnEquipCrossbow_Callback(Crossbow crossbow)
    {
        OnActivateCrosshair?.Invoke();
        _crossbowCrosshair.Activate(crossbow);
    }

    void OnEquipHammer_Callback(Sword hammer)
    {
        OnActivateCrosshair?.Invoke();
        _hammerCrosshair.Activate(hammer);
    }

    void DisableBaseCrosshair()
    {
        _baseCrosshair.SetActive(false);
    }
}
