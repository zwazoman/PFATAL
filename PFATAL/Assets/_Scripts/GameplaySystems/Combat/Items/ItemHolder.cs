using UnityEngine;
using Unity.Netcode;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEditor;
using _scripts.PlayerCharacter;

public class ItemHolder : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] Hand _leftHand;
    [SerializeField] Hand _rightHand;

    [Header("Settings")]

    [SerializeField] public List<GameObject> itemPrefabs;

    Dictionary<string, Item> _itemsDict = new();

    Item _leftItem = null;
    Item _rightItem = null;

    private void Start()
    {
        foreach (GameObject prefab in itemPrefabs)
        {
            GameObject itemObject = Instantiate(prefab, _leftHand.transform.parent);
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
        currentItem.transform.parent = hand.visualsTransform;
        currentItem.transform.position = hand.visualsTransform.position;
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
        currentItem.transform.parent = _leftHand.transform.parent;
    }

    Hand GetHand(bool isLeft)
    {
        if (isLeft)
            return _leftHand;
        else
            return _rightHand;
    }
}
