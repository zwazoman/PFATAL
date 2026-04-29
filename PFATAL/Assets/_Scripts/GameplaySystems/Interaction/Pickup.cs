using DG.Tweening;
using System;
using Unity.Netcode;
using UnityEngine;

public class Pickup : Interactable
{
    public event Action OnOnlinePickup;
    public event Action OnPickup;

    [SerializeField] bool _despawnsOnPickup = true;

    [Header("References")]
    [SerializeField] GameObject _visuals;

    [Header("Item Info")]
    [SerializeField] ItemInfo _itemInfo;

    [Header("Tweens")]
    [SerializeField] float _pickupTweenScale  = 1.2f;
    [SerializeField] float _pickupTweenDuration = .5f;

    [Header("Settings")]
    [SerializeField] float _lifeTime = 5f;

    [HideInInspector] public bool despawns = true;
    [HideInInspector] public bool pickedUp = false;


    float _timer;

    public override void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        if (pickedUp)
            return;

        if (interaction._playerCharacter.playerHands.TryEquipItem(_itemInfo))
        {
            OnPickup?.Invoke();

            _visuals.SetActive(false);

            PickupRpc();

            if (_despawnsOnPickup)
                DespawnRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    void PickupRpc()
    {
        OnOnlinePickup?.Invoke();
        pickedUp = true;
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

    public ItemInfo(ItemType itemType, GameObject itemPrefab)
    {
        this.itemType = itemType;
        this.itemPrefab = itemPrefab;
    }
}