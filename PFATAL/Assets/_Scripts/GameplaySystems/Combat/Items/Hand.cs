using _scripts.PlayerCharacter;
using System.Collections.Generic;
using UnityEngine;

public class Hand: MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerCharacter _main;
    [SerializeField] ItemVisuals _itemVisuals;

    [Header("Parameters")]

    [SerializeField] public ItemType type;

    [SerializeField] int _inventorySize = 1;

    [HideInInspector] public ItemScriptable heldItem;
    [HideInInspector] public List<ItemScriptable> itemSlots = new();


    public void PickupItem(ItemScriptable item)
    {
        itemSlots.Add(item);

        if (heldItem == null)
            heldItem = item;

        item.OnPickup(ref _main);
        EquipItem(item);
    }

    public bool TryPickupItem(ItemScriptable item)
    {
        if(itemSlots.Count < _inventorySize)
        {
            PickupItem(item);
            return true;
        }

        return false;
    }

    void EquipItem(ItemScriptable item)
    {
        //animation

        heldItem = item;

        _itemVisuals.ShowItemRpc(item.mesh.name);
        heldItem.OnEquip();
    }

    public void SwitchToPreviousHeldItem()
    {
        print(gameObject + " try next item");
        if(itemSlots.Count > 1)
        {
            print("next Item");
            EquipItem(itemSlots.GetPreviousObjectWrapped(heldItem));
        }
    }

    public void SwitchToNextHeldItem()
    {
        print(gameObject + " try previous item");

        if (itemSlots.Count > 1)
        {
            print("previous Item");
            EquipItem(itemSlots.GetNextObjectWrapped(heldItem));
        }

    }

    public void DropItem()
    {
        print(gameObject + " drop");
    }
}
