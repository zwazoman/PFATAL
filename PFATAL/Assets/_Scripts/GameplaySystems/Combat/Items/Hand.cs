using _scripts.PlayerCharacter;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerCharacter _main;
    [SerializeField] ItemVisuals _itemVisuals;
    [SerializeField] public Transform visualsTransform;

    [Header("Parameters")]

    [SerializeField] bool _isLeft;
    [SerializeField] public ItemType type;

    [SerializeField] int _inventorySize = 1;

    [HideInInspector] public Item equippedItem;
    [HideInInspector] List<Item> itemInventory = new();

    /// <summary>
    /// vérifie si un item est ramassable en fonction de l'item info. si il est bien ramassable : le ramasse
    /// </summary>
    /// <param name="itemInfo"></param>
    /// <returns></returns>
    public bool TryPickupItem(ItemInfo itemInfo)
    {
        if (itemInventory.Count < _inventorySize)
        {
            Item item = _itemVisuals.GetItem(itemInfo.itemPrefab.name);

            itemInventory.Add(item);
            item.OnPickup(_main, this);
            EquipItem(item);

            return true;
        }
        return false;
    }

    /// <summary>
    /// définit "item" comme l'item porté par la main et l'affiche au yeux de tous les joueurs.
    /// </summary>
    /// <param name="item"></param>
    void EquipItem(Item item)
    {
        //animation

        if (!itemInventory.Contains(item))
        {
            print("item not pickedUp");
            return;
        }

        if (equippedItem != null)
        {
            UnEquipEquippedItem();
        }

        equippedItem = item;
        _itemVisuals.ShowItemRpc(item.gameObject.name, _isLeft);

        equippedItem.OnEquip();
    }

    /// <summary>
    /// appelle "OnDrop" sur l'item équipé puis, le retire de la main et définit l'item précédent de la liste comme le nouveau dans la main
    /// </summary>
    public void DropHeldItem()
    {
        if (equippedItem == null)
            return;

        equippedItem.OnDrop();
        DeleteItem(equippedItem);
    }

    /// <summary>
    /// définit le prochain ou le précédent (en fonction de "isPrevious") item de la liste d'items comme celui équipé
    /// </summary>
    /// <param name="isPrevious"></param>
    /// <returns></returns>
    public void SwitchEquippedItem(bool isPrevious)
    {
        if(equippedItem == null || itemInventory.Count <= 0)
            return;

        Item oldHeldItem = equippedItem;

        if (isPrevious)
            EquipItem(itemInventory.GetPreviousObjectWrapped(equippedItem)); // previous Item
        else
            EquipItem(itemInventory.GetNextObjectWrapped(equippedItem)); // next item

    }

    /// <summary>
    /// retire l'item actuellement porté de la main et update le visuel pour les autres joueurs
    /// </summary>
    public void UnEquipEquippedItem()
    {
        equippedItem.OnUnEquip();
        _itemVisuals.HideEquippedItemRpc(_isLeft);
        equippedItem = null;
    }

    /// <summary>
    /// retire "item" de l'inventaire. le déséqippe également si il est équipé.
    /// </summary>
    /// <param name="item"></param>
    public void DeleteItem(Item item)
    {
        Item oldEquippedOtem = equippedItem;

        if (itemInventory.Count > 1)
            SwitchEquippedItem(true);
        else
            UnEquipEquippedItem();

        itemInventory.Remove(oldEquippedOtem);
    }

    public void DeleteEquippedItem()
    {
        DeleteItem(equippedItem);
    }
}