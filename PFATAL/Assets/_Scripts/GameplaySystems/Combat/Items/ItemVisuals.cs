using UnityEngine;
using Unity.Netcode;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEditor;

public class ItemVisuals : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Hand _leftHand;
    [SerializeField] Hand _rightHand;

    [Header("Settings")]

    [SerializeField] List<GameObject> _prefabs;


    public Item equippedItem = null;
    Dictionary<string, Item> itemsDict = new();

    private void Start()
    {
        foreach (GameObject prefab in _prefabs)
        {
            GameObject itemObject = Instantiate(prefab, _leftHand.transform.parent);
            itemObject.name = prefab.name;

            if(itemObject.TryGetComponent(out Item item))
            {
                itemsDict.Add(prefab.name, item);
            }
            else
                Debug.LogError($"{prefab.name} does not contain an Item component.");

            itemObject.SetActive(false);
        }
    }

    public Item GetItem(string prefabName)
    {
        return itemsDict[prefabName];
    }

    [Rpc(SendTo.Everyone)]
    public void ShowItemRpc(string prefabName, bool leftHand)
    {
        if (!itemsDict.ContainsKey(prefabName))
        {
            Debug.LogError($"{prefabName} not found in ItemsDictionary");
            return;
        }

        Hand hand = GetHand(leftHand);

        Item currentItem =  GetItem(prefabName);
        currentItem.transform.parent = hand.visualsTransform;
        currentItem.transform.position = hand.visualsTransform.position;
        currentItem.gameObject.SetActive(true);
    }

    [Rpc(SendTo.Everyone)]
    public void HideEquippedItemRpc(bool leftHand)
    {
        Hand hand = GetHand(leftHand);

        hand.equippedItem.gameObject.SetActive(false);
        hand.equippedItem.transform.parent = _leftHand.transform.parent;
    }

    Hand GetHand(bool isLeft)
    {
        if (isLeft)
            return _leftHand;
        else
            return _rightHand;
    }
}
