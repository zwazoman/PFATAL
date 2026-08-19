using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HUDManager _hud;
    [SerializeField] Image _equippedItemIcon;
    [SerializeField] Image _unequippedItemIcon;

    Hand leftHand;

    private void Start()
    {
        print("oui");
        ClearItems();

        leftHand = _hud.playerCharacter.playerHands.leftHand;

        leftHand.OnDeleteItem += ClearItems;

        leftHand.OnEquipItem += UpdateCurrentEquipped;
        leftHand.OnUnequipItem += UpdateCurrentUnequipped;
    }

    void UpdateCurrentEquipped(Item newItem)
    {
        print("faut afficher là");
        _equippedItemIcon.sprite = newItem.uiSprite;
        //_equippedItemIcon.transform.DOPunchScale(new Vector3(2, 2, 1), .4f).SetEase(Ease.InCubic) ;
    }

    void UpdateCurrentUnequipped(Item oldItem)
    {
        _unequippedItemIcon.sprite = oldItem.uiSprite;
    }

    void ClearItems()
    {
        if (leftHand == null || leftHand.itemInventory == null)
            return;

        if (leftHand.itemInventory.Count != 1)
            _unequippedItemIcon.sprite = null;

        _equippedItemIcon.sprite = null;
    }
}
