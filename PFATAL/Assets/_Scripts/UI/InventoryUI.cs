using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Hand _leftHand;
    [SerializeField] TMP_Text _inventoryText;

    private void Start()
    {
        ClearText();

        _leftHand.OnUnequipItem += EditText;
        _leftHand.OnDeleteItem += ClearText;
    }

    void ClearText()
    {
        _inventoryText.text = string.Empty;
    }

    void EditText(Item item)
    {
        if (_leftHand.itemInventory.Count >= 1)
            _inventoryText.text = item.gameObject.name;
        else
            ClearText();
    }
}
