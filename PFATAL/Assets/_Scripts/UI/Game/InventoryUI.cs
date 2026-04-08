using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HUDManager _hud;
    [SerializeField] TMP_Text _inventoryText;

    Hand leftHand;

    private void Start()
    {
        ClearText();

        leftHand = _hud.playerCharacter.playerHands.leftHand;

        leftHand.OnUnequipItem += EditText;
        leftHand.OnDeleteItem += ClearText;
    }

    void ClearText()
    {
        _inventoryText.text = string.Empty;
    }

    void EditText(Item item)
    {
        if (leftHand.itemInventory.Count >= 1)
            _inventoryText.text = item.gameObject.name;
        else
            ClearText();
    }
}
