using DG.Tweening;
using System;
using Unity.Netcode;
using UnityEngine;

public class Pickup : Interactable
{
    public event Action OnPickup;

    [SerializeField] bool _despawnsOnPickup = true;

    [Header("Item Info")]
    [SerializeField] ItemInfo _itemInfo;

    [Header("Tweens")]
    [SerializeField] float _pickupTweenScale  = 1.2f;
    [SerializeField] float _pickupTweenDuration = .5f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override async void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        if (interaction.main.playerHands.TryEquipItem(_itemInfo))
        {
            transform.DOPunchScale(transform.localScale * _pickupTweenScale, _pickupTweenDuration,0,0);
            PickupRpc();

            if (_despawnsOnPickup)
                DespawnRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    void PickupRpc()
    {
        OnPickup?.Invoke();
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
