using System;
using Unity.Netcode;
using UnityEngine;

public class Pickup : Interactable
{
    [SerializeField] bool _despawnsOnPickup = true;

    [Header("Item Info")]
    [SerializeField] ItemInfo _itemInfo;

    public override void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        if(interaction.main.playerHands.TryEquipItem(_itemInfo))
            if(_despawnsOnPickup)
                DespawnRpc();
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
