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

            itemObject.SetActive(false);
        }
    }

    public Item GetItem(string prefabName)
    {
        return _itemsDict[prefabName];
    }

    [Rpc(SendTo.Everyone)]
    public void ShowItemRpc(string prefabName, bool leftHand)
    {
        if (!_itemsDict.ContainsKey(prefabName))
        {
            Debug.LogError($"{prefabName} not found in ItemsDictionary");
            return;
        }

        Hand hand = GetHand(leftHand);

        Item currentItem =  GetItem(prefabName);
        currentItem.enabled = _playerCharacter.IsOwner;
        currentItem.transform.parent = hand._itemSocket;
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.transform.localScale = Vector3.one;
        currentItem.gameObject.SetActive(true);

        if (leftHand)
            _leftItem = currentItem;
        else
            _rightItem = currentItem;
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
