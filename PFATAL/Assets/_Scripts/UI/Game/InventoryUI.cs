using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HUDManager _hud;
    [SerializeField] Image _inventoryIcon;

    Hand leftHand;

    private void Start()
    {
        print("oui");
        ClearText();

        leftHand = _hud.playerCharacter.playerHands.leftHand;

        leftHand.OnUnequipItem += EditText;
        leftHand.OnDeleteItem += ClearText;
    }

    void ClearText()
    {
        _inventoryIcon.enabled = false;
    }

    void EditText(Item item)
    {
        print("UnequipItem");

        if (leftHand.itemInventory.Count >= 1)
        {
            _inventoryIcon.enabled = true;
             if(item.uiSprite != null) 
                _inventoryIcon.sprite = item.uiSprite;
        }
        else
            ClearText();
    }
}
