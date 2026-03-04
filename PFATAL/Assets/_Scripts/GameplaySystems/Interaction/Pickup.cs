using System;
using Unity.Netcode;
using UnityEngine;

public class Pickup : Interactable
{
    [Header("Item Info")]
    [SerializeField] ItemInfo _itemInfo;

    public override void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        if(interaction.main.playerHands.TryEquipItem(_itemInfo))
            DespawnRpc();
        else
            print("couldn't equip item");
    }

    [Rpc(SendTo.Server)]
    void DespawnRpc()
    {
        NetworkObject.Despawn();
    }
}

[Serializable]
public struct ItemInfo
{
    public ItemType itemType;
    public GameObject itemPrefab;
}
