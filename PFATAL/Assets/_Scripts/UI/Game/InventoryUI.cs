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

    Tween punchTween = null;
    bool fading;

    private void Start()
    {
        _unequippedItemIcon.enabled = false;
        _equippedItemIcon.enabled = false;

        leftHand = _hud.playerCharacter.playerHands.leftHand;

        leftHand.OnDeleteItem += ClearItems;

        leftHand.OnEquipItem += UpdateCurrentEquipped;
        leftHand.OnUnequipItem += UpdateCurrentUnequipped;
    }

    void UpdateCurrentEquipped(Item newItem)
    {
        fading = false;

        Color c = _equippedItemIcon.color;
        c.a = 1;
        _equippedItemIcon.color = c;

        _equippedItemIcon.transform.localScale = Vector3.one;

        _equippedItemIcon.enabled = true;
        _equippedItemIcon.sprite = newItem.uiSprite;

        punchTween.Kill();
        punchTween = _equippedItemIcon.transform.DOPunchScale(new Vector3(.8f, .8f, 0), .6f, 2)
            .SetEase(Ease.OutCubic);     
    }

    void UpdateCurrentUnequipped(Item oldItem)
    {
        _unequippedItemIcon.enabled = true;
        _unequippedItemIcon.sprite = oldItem.uiSprite;
    }

    void ClearItems()
    {
        if (leftHand == null || leftHand.itemInventory == null)
            return;

        _unequippedItemIcon.enabled = false;

        if (leftHand.itemInventory.Count == 0)
        {
            fading = true;
            //_equippedItemIcon.DOFade(0, .2f).OnComplete(() => _equippedItemIcon.enabled = false);
            Tween fadeTween = null;

            fadeTween = _equippedItemIcon.DOFade(0, .4f)
                .SetEase(Ease.InCubic)
                .OnUpdate(() =>
            {
                if (!fading)
                    fadeTween.Kill();
            })
                .OnComplete(() =>
                {
                    _equippedItemIcon.enabled = false;
                });
        }
    }
}
