using _scripts.PlayerCharacter;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public event Action<Item> OnPickUpItem;
    public event Action<Item> OnDropItem;
    public event Action OnDeleteItem;

    public event Action<Item> OnEquipItem;
    public event Action<Item> OnUnequipItem;

    public event Action<Crossbow> OnEquipCrossbow;
    public event Action<Tomahawk> OnEquipTomahawk;
    public event Action<Hammer> OnEquipHammer;

    public event Action OnSwapItem;

    [Header("References")]
    [SerializeField] PlayerCharacter _main;
    [SerializeField] ItemHolder _itemVisuals;
    [SerializeField] public Transform visualsTransform;
    [SerializeField] Animator _animator;

    [Header("Parameters")]

    [SerializeField] bool _isLeft;
    [SerializeField] public ItemType type;
    [SerializeField] bool _swapWhenFull;

    [SerializeField] int _inventorySize = 1;

    [HideInInspector] public Item equippedItem;
    [HideInInspector] public List<Item> itemInventory = new();

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
            itemInventory.Add(item);
            item.Pickup(_main, this);
            EquipItem(item);

            OnPickUpItem?.Invoke(item);
            return true;
        }
        else if (_swapWhenFull)
        {
            DeleteEquippedItem();

            itemInventory.Add(item);
            item.Pickup(_main, this);

            EquipItem(item);

            OnPickUpItem?.Invoke(item);
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
        if (!itemInventory.Contains(item))
        {
            print("item not pickedUp");
            return;
        }

        OnEquipItem?.Invoke(item);

        _animator.SetTrigger("Equip");


        if (equippedItem != null)
        {
            print(equippedItem.name);
            UnEquipItem();
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

        OnDropItem?.Invoke(equippedItem);

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
        if(equippedItem == null || itemInventory.Count <= 1)
            return;

        OnSwapItem?.Invoke();

        Item oldHeldItem = equippedItem;

        if (isPrevious)
            EquipItem(itemInventory.GetPreviousObjectWrapped(equippedItem)); // previous Item
        else
            EquipItem(itemInventory.GetNextObjectWrapped(equippedItem)); // next item

    }

    /// <summary>
    /// retire l'item actuellement porté de la main et update le visuel pour les autres joueurs
    /// </summary>
    public void UnEquipItem()
    {
        print("unequip item " + equippedItem.gameObject.name);

        OnUnequipItem?.Invoke(equippedItem);

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
            UnEquipItem();

        OnDeleteItem?.Invoke();

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

    public void ClearInventory()
    {
        List<Item> tmpItems = new();
        tmpItems.AddRange(itemInventory);

        foreach (Item item in tmpItems)
            DeleteItem(item);
    }

    #region specific equips

    public void EquipSpecific(Hammer hammer) { OnEquipHammer?.Invoke(hammer); }
    public void EquipSpecific(Crossbow crossbow) { OnEquipCrossbow?.Invoke(crossbow); }
    public void EquipSpecific(Tomahawk tomahawk) { OnEquipTomahawk?.Invoke(tomahawk); }

    #endregion
}