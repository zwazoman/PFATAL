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
    public bool TryPickupItem(Item item, GameObject itemPrefab)
    {
        if (itemInventory.Count < _inventorySize)
        {
            print(item.type);
            print(item.prefabName);

            GameObject itemObject = _itemVisuals.ShowItem(item.prefabName, _isLeft);
            Item itemComponent = itemObject.GetComponent<Item>();
            itemInventory.Add(itemComponent);
            item.OnPickup(_main, this);
            EquipItem(itemComponent);

            return true;
        }
        return false;
    }

    void EquipNewItem(GameObject itemPrefab)
    {
        //instancie le prefab
        //récupère l'item
        //l'ajoute a l'inventaire
        //appelle OnPickup dessus
        //EquipItem()
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

        equippedItem.OnEquip();
    }

    /// <summary>
    /// appelle "OnDrop" sur l'item équipé puis, le retire de la main et définit l'item précédent de la liste comme le nouveau dans la main
    /// </summary>
    public void DropHeldItem()
    {
        if (equippedItem == null)
        {
            print("y'a rien à drop dans ta main ducon");
            return;
        }

        print(gameObject + " drop");

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
        {
            print("next Item");
            EquipItem(itemInventory.GetPreviousObjectWrapped(equippedItem));
        }
        else
        {
            print("previous Item");
            EquipItem(itemInventory.GetNextObjectWrapped(equippedItem));
        }
    }

    /// <summary>
    /// retire l'item actuellement porté de la main et update le visuel pour les autres joueurs
    /// </summary>
    public void UnEquipEquippedItem()
    {
        //_itemVisuals.HideItemRpc(_isLeft);
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