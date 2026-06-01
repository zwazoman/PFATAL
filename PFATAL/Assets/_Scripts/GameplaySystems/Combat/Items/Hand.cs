using _scripts.PlayerCharacter;
using System;
using System.Collections.Generic;
using GameplaySystems.PlayerCharacter;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Hand : MonoBehaviour
{
    //events, pas repliqués
    public event Action<Item> OnPickUpItem;
    public event Action<Item> OnDropItem;
    public event Action OnDeleteItem;
    public event Action<Item> OnEquipItem;
    public event Action<Item> OnUnequipItem;
    public event Action<Crossbow> OnEquipCrossbow;
    public event Action<Tomahawk> OnEquipTomahawk;
    public event Action<Sword> OnEquipHammer;
    public event Action OnSwapItem;

    [Header("References")]
    [FormerlySerializedAs("_main")] public  PlayerCharacter playerCharacter;
    public HandsItemVisuals itemVisuals;
    public PlayerHandVisuals visuals;
    public HandAnimatorEventListener animatorEventListener;
    public Transform _itemSocket;

    [Header("Parameters")]

    [SerializeField] bool _isLeft;
    [SerializeField] public ItemType type;
    [SerializeField] bool _swapWhenFull;

    [SerializeField] int _inventorySize = 1;

    [SerializeField] public Item equippedItem;
    [SerializeField] public List<Item> itemInventory = new();

    public Vector3 fpsPosition, tpsPosition;

    private bool swordsUsed = false;
    private bool tomahawksUsed = false;
    private bool crossbowsUsed = false;

    /// <summary>
    /// v�rifie si un item est ramassable en fonction de l'item info. si il est bien ramassable : le ramasse
    /// </summary>
    /// <param name="itemInfo"></param>
    /// <returns></returns>
    public bool TryPickupItem(ItemInfo itemInfo)
    {
        Item item = itemVisuals.GetItemInstance(itemInfo.itemPrefab.name);

        if (itemInventory.Count < _inventorySize)
        {
            itemInventory.Add(item);
            item.Pickup(playerCharacter, this);
            EquipItem(item);

            OnPickUpItem?.Invoke(item);
            return true;
        }
        else if (_swapWhenFull)
        {
            DeleteEquippedItem();

            itemInventory.Add(item);
            item.Pickup(playerCharacter, this);

            EquipItem(item);

            OnPickUpItem?.Invoke(item);
            return true;
        }

        return false;
    }

    /// <summary>
    /// d�finit "item" comme l'item port� par la main et l'affiche au yeux de tous les joueurs.
    /// </summary>
    /// <param name="item"></param>
    void EquipItem(Item item)
    {
        if (!itemInventory.Contains(item))
        {
            print("item not pickedUp");
            return;
        }
        
        if (equippedItem != null)
        {
            print(equippedItem.name);
            UnEquipItem();
        }
        
        equippedItem = item;
        OnEquipItem?.Invoke(item);
        itemVisuals.ShowItemRpc(item.gameObject.name, _isLeft);

        equippedItem.Equip();

        if (item is Sword sword)
        {
            EquipSpecific(sword);
            swordsUsed = true;
        }

        if (item is Tomahawk tomahawk)
        {
            EquipSpecific(tomahawk);
            tomahawksUsed = true;
        }

        if (item is Crossbow crossbow)
        {
            EquipSpecific(crossbow);
            crossbowsUsed = true;
        }
    }

    /// <summary>
    /// appelle "OnDrop" sur l'item �quip� puis, le retire de la main et d�finit l'item pr�c�dent de la liste comme le nouveau dans la main
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
    /// d�finit le prochain ou le pr�c�dent (en fonction de "isPrevious") item de la liste d'items comme celui �quip�
    /// </summary>
    /// <param name="isPrevious"></param>
    /// <returns></returns>
    public void ScrollEquippedItem(bool isPrevious)
    {
        if(equippedItem == null || itemInventory.Count <= 1)
            return;

        OnSwapItem?.Invoke();

        //Item oldHeldItem = equippedItem;
        
        if (isPrevious)
            EquipItem(itemInventory.GetPreviousObjectWrapped(equippedItem)); // previous Item
        else
            EquipItem(itemInventory.GetNextObjectWrapped(equippedItem)); // next item

    }

    /// <summary>
    /// retire l'item actuellement port� de la main et update le visuel pour les autres joueurs
    /// </summary>
    public void UnEquipItem()
    {
        print("unequip item " + equippedItem.gameObject.name);

        OnUnequipItem?.Invoke(equippedItem);

        equippedItem.UnEquip();
        itemVisuals.HideEquippedItemRpc(_isLeft);
        equippedItem = null;
    }

    /// <summary>
    /// retire "item" de l'inventaire. le d�s�qippe �galement si il est �quip�.
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
    /// drop l'item actuel et en �quipe un nouveau
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

    public void EquipSpecific(Sword hammer) { OnEquipHammer?.Invoke(hammer); }
    public void EquipSpecific(Crossbow crossbow) { OnEquipCrossbow?.Invoke(crossbow); }
    public void EquipSpecific(Tomahawk tomahawk) { OnEquipTomahawk?.Invoke(tomahawk); }

    #endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(Hand))]
class HandEditor : Editor
{
    override public void OnInspectorGUI()
    {
        Hand t = (Hand)target;
        base.OnInspectorGUI();
        
        GUILayout.Space(10);
        GUILayout.Label("fps position : "+t.fpsPosition);
        if(GUILayout.Button("save current position as FPS Position"))
            t.fpsPosition = t.transform.localPosition;
        if(GUILayout.Button("go to FPS position"))
            t.transform.localPosition = t.fpsPosition;
        
        GUILayout.Space(5);
        GUILayout.Label("tps position : "+t.tpsPosition);
        if(GUILayout.Button("save current position as TPS Position"))
            t.tpsPosition = t.transform.localPosition;
        if(GUILayout.Button("go to TPS position"))
            t.transform.localPosition = t.tpsPosition;
    }
}

#endif