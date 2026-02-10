using System.Collections.Generic;
using UnityEngine;

public class Hand: MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMain _main;
    [SerializeField] ItemVisuals _itemVisuals;

    [Header("Parameters")]

    [SerializeField] public ItemType type;

    [SerializeField] int _inventorySize = 1;

    [HideInInspector] public ItemScriptable heldItem;
    [HideInInspector] public List<ItemScriptable> itemSlots = new();

    private void Update()
    {
        //if(heldItem != null)
        //{
        //    heldItem.transform.SetPositionAndRotation(_holdingSocket.position, _holdingSocket.rotation);
        //}
    }

    public void PickupItem(ItemScriptable item)
    {
        itemSlots.Add(item);

        if (heldItem == null)
            heldItem = item;

        item.OnPickup(ref _main);
        EquipItem(item);

        //print(heldItem.NetworkObject);
        //print(heldItem.IsSpawned);

        //print("ask for ownership");
        //TryChangeOwnershipRpc(NetworkManager.Singleton.LocalClientId, heldItem.NetworkObject);
        //await WaitForOwnership(NetworkManager.Singleton.LocalClientId, heldItem);

        //item.transform.parent = _main.transform;
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
        EquipItem(itemSlots.GetPreviousObjectWrapped(heldItem));
    }

    public void SwitchToNextHeldItem()
    {
        EquipItem(itemSlots.GetNextObjectWrapped(heldItem));
    }

    public void DropItem()
    {
        //if (heldItem == null)
        //    return;

        //itemSlots.Remove(heldItem);

        //heldItem.transform.parent = null;

        //heldItem.OnDrop();

        //heldItem.NetworkObject.ChangeOwnership(0);
        //heldItem = null;
    }

    //todo : extension networkObj
}
