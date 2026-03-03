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
    [HideInInspector] public List<Item> itemInventory = new();

    /// <summary>
    /// essaye de ramasser un item en fonction de la place présente dans l'inventaire et l'équipe si possible. retourne le résultat.
    /// </summary>
    /// <param name="item"></param>
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
        {
            print("not enough items to scroll into");
            return;
        }

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
        _itemVisuals.HideEquippedItemRpc(_isLeft);
        equippedItem = null;
    }

    /// <summary>
    /// retire un item présent dans l'inventaire du joueur
    /// </summary>
    /// <param name="item"></param>
    public void UnEquipItem(Item item)
    {
        if (!itemInventory.Contains(item))
            return;

        if (equippedItem == item)
            UnEquipEquippedItem();
        else
            itemInventory.Remove(item);
    }

    public void DeleteItem(Item item)
    {
        Item oldEquippedOtem = equippedItem;

        if (itemInventory.Count > 1)
        {
            SwitchEquippedItem(true);
        }
        else
        {
            UnEquipEquippedItem();
        }

        itemInventory.Remove(oldEquippedOtem);
    }
}