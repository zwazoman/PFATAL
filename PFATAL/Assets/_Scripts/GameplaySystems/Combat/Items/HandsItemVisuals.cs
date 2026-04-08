using UnityEngine;
using Unity.Netcode;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEditor;
using _scripts.PlayerCharacter;

public class HandsItemVisuals : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] PlayerHands _hands;

    [Header("Settings")]
    [SerializeField] public List<GameObject> itemPrefabs;

    Dictionary<string, Item> _itemsDict = new();

    Item _leftItem = null;
    Item _rightItem = null;

    private void Start()
    {
        //instantiate item prefabs in hands
        foreach (GameObject prefab in itemPrefabs)
        {
            GameObject itemObject = Instantiate(prefab, _hands.transform.parent);
            itemObject.name = prefab.name;
            
            if(itemObject.TryGetComponent(out Item item))
            {
                _itemsDict.Add(prefab.name, item);
            }
            else
                Debug.LogError($"{prefab.name} does not contain an Item component.");
            
            //...and hide them
            itemObject.SetActive(false);
        }
    }
    
    public Item GetItemInstance(string prefabName)
    {
        return _itemsDict[prefabName];
    }

    
    [Rpc(SendTo.Everyone)]
    public void ShowItemRpc(string prefabName, bool leftHand)
    {
        //look for item instance
        if (!_itemsDict.ContainsKey(prefabName))
        {
            Debug.LogError($"{prefabName} not found in ItemsDictionary");
            return;
        }

        //get hand
        Hand hand = GetHand(leftHand);
        
        //show item and attach it to the requested hand 
        Item item =  GetItemInstance(prefabName);
        item.enabled = true;
        item.transform.parent = hand._itemSocket;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;
        item.gameObject.SetActive(true);

        //change the item's layer to match the hand's layer
        void SetLayerRecursive(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
            {
                SetLayerRecursive(child, layer);
            }
        }
        SetLayerRecursive(item.transform,hand.gameObject.layer);
        
        (leftHand? ref _leftItem : ref _rightItem) = item;
        //
        // if (leftHand)
        //     _leftItem = item;
        // else
        //     _rightItem = item;
    }

    [Rpc(SendTo.Everyone)]
    public void HideEquippedItemRpc(bool leftHand)
    {
        Item currentItem;

        if (leftHand)
            currentItem = _leftItem;
        else
            currentItem = _rightItem;

        currentItem.gameObject.SetActive(false);
        currentItem.transform.parent = _hands.transform.parent;
    }

    Hand GetHand(bool isLeft)
    {
        return isLeft ? _hands.leftHand : _hands.rightHand;
    }
}
