using UnityEngine;
using Unity.Netcode;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEditor;

public class ItemVisuals : NetworkBehaviour
{
    [Header("left Hand")]
    [SerializeField] Hand _leftHand;

    [Header("Right Hand")]
    [SerializeField] Hand _rightHand;

    [Header("Prefabs")]

    [SerializeField] List<GameObject> _prefabs;

    Dictionary<string, GameObject> itemPrefabsDict = new();

    Dictionary<string, GameObject> equippedPrefabs = new();

    private void Awake()
    {
        foreach (GameObject prefab in _prefabs)
        {
            itemPrefabsDict.Add(prefab.name, prefab);
            print(prefab.name + " added to dictionary");
        }
    }

    public GameObject ShowItem(string prefabName, bool leftHand)
    {
        Hand hand;

        if(leftHand)
            hand = _leftHand;
        else
            hand = _rightHand;

        print(prefabName);

        ShowItemRpc(prefabName, leftHand);

        return Instantiate(itemPrefabsDict[prefabName], hand.visualsTransform.position, hand.visualsTransform.rotation);


    }

    [Rpc(SendTo.NotMe)]
    void ShowItemRpc(string objectName, bool leftHand = true)
    {
        Hand hand;

        if(leftHand)
            hand = _leftHand;
        else
            hand = _rightHand;

        Instantiate(itemPrefabsDict[objectName], hand.visualsTransform.position, hand.visualsTransform.rotation);
    }

    [Rpc(SendTo.Everyone)]
    public void HideItemRpc(bool leftHand = true)
    {

    }
}

[Serializable]
public class ItemVisual
{
    public Mesh mesh;
    public Vector3 positionOffset;
    public Vector3 rotation;
    public float scaleMultiplyer = 1;
}
