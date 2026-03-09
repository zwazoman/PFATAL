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

    NetworkVariable<bool> _isPickup = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsServer)
            _isPickup.Value = false;
    }

    public override async void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        if (_isPickup.Value)
            return;

        if (interaction.main.playerHands.TryEquipItem(_itemInfo))
        {
            _isPickup.Value = true;
            transform.DOPunchScale(transform.localScale * _pickupTweenScale, _pickupTweenDuration,0,0).onComplete += PickupEnd;

            //if (_despawnsOnPickup)
            //    DespawnRpc();
        }
        //OnPickup?.Invoke();
    }

    void PickupEnd()
    {
        if (_despawnsOnPickup)
            DespawnRpc();

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
