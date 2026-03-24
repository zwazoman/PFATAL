using _scripts.PlayerCharacter;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public event Action<Item> OnItemPickedUp;
    public event Action OnItemDropped;

    public event Action OnItemSwapped;

    [Header("References")]
    [SerializeField] PlayerCharacter _main;
    [SerializeField] ItemVisuals _itemVisuals;
    [SerializeField] public Transform visualsTransform;

    [Header("Parameters")]

    [SerializeField] bool _isLeft;
    [SerializeField] public ItemType type;
    [SerializeField] bool _swapWhenFull;

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
        Item item = _itemVisuals.GetItem(itemInfo.itemPrefab.name);

        if (itemInventory.Count < _inventorySize)
        {
            OnItemPickedUp?.Invoke(item);

            itemInventory.Add(item);
            item.Pickup(_main, this);
            EquipItem(item);
            return true;
        }
        else if (_swapWhenFull)
        {
            OnItemPickedUp?.Invoke(item);

            itemInventory.Add(item);
            item.Pickup(_main, this);
            SwapAndDropEquippedItem(item);
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

        equippedItem.Equip();
    }

    /// <summary>
    /// appelle "OnDrop" sur l'item équipé puis, le retire de la main et définit l'item précédent de la liste comme le nouveau dans la main
    /// </summary>
    public void DropEquippedtem()
    {
        if (equippedItem == null)
            return;

        OnItemDropped?.Invoke();

        equippedItem.Drop();
        DeleteEquippedItem();
    }

    /// <summary>
    /// définit le prochain ou le précédent (en fonction de "isPrevious") item de la liste d'items comme celui équipé
    /// </summary>
    /// <param name="isPrevious"></param>
    /// <returns></returns>
    public void ScrollEquippedItem(bool isPrevious)
    {
        if(equippedItem == null || itemInventory.Count <= 0)
            return;

        OnItemSwapped?.Invoke();

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
        equippedItem.UnEquip();
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
            ScrollEquippedItem(true);
        else
            UnEquipEquippedItem();

        itemInventory.Remove(oldEquippedOtem);
    }

    public void DeleteEquippedItem()
    {
        DeleteItem(equippedItem);
    }

    /// <summary>
    /// drop l'item actuel et en équipe un nouveau
    /// </summary>
    /// <param name="item"></param>
    public void SwapAndDropEquippedItem(Item item)
    {
        DropEquippedtem();
        EquipItem(item);
    }

    public void SwapAndDeleteEquippedItem(Item item)
    {
        DeleteEquippedItem();
        EquipItem(item);
    }
}