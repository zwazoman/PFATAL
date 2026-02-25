using _scripts.PlayerCharacter;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerCharacter _main;
    [SerializeField] ItemVisuals _itemVisuals;

    [Header("Parameters")]

    [SerializeField] bool _isLeft;
    [SerializeField] public ItemType type;

    [SerializeField] int _inventorySize = 1;

    [HideInInspector] public ItemScriptable heldItem;
    [HideInInspector] public List<ItemScriptable> itemSlots = new();

    public bool TryPickupItem(ItemScriptable item)
    {
        if (itemSlots.Count < _inventorySize)
        {
            itemSlots.Add(item);
            item.OnPickup(ref _main);
            EquipItem(item);

            return true;
        }
        return false;
    }

    void EquipItem(ItemScriptable item)
    {
        //animation

        if (!itemSlots.Contains(item))
        {
            print("item not pickedUp");
            return;
        }

        if (heldItem != null)
        {
            UnEquipHeldItem();
        }

        heldItem = item;

        _itemVisuals.ShowItemRpc(item.mesh.name, _isLeft);
        heldItem.OnEquip();
    }

    void UnEquipHeldItem()
    {
        _itemVisuals.HideItemRpc(_isLeft);
        heldItem = null;
    }

    public void DropHeldItem()
    {
        if (heldItem == null)
        {
            print("y'a rien à drop dans ta main ducon");
            return;
        }

        print(gameObject + " drop");

        heldItem.OnDrop();

        int oldItemIndex = itemSlots.IndexOf(heldItem);
        itemSlots.Remove(heldItem);
        UnEquipHeldItem();

        if(itemSlots.Count > 0)
        {
            if (oldItemIndex == 0)
                EquipItem(itemSlots[itemSlots.Count - 1]);
            else
                EquipItem(itemSlots[oldItemIndex - 1]);
        }
    }

    public void SwitchToPreviousHeldItem()
    {
        print(gameObject + " try next item");
        if (itemSlots.Count > 1)
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
}
