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

    [Header("Settings")]
    [SerializeField] float _lifeTime = 5f;

    [HideInInspector] public bool despawns = true;
    [HideInInspector] public bool pickedUp = false;


    float _timer;

    private void Update()
    {
        //if (!IsServer && !despawns)
        //    return;

        //_timer += Time.deltaTime;

        //if(_timer > _lifeTime)
        //{
        //    _timer = 0;
        //    DespawnRpc();
        //}
    }

    public override void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        if (pickedUp)
            return;

        if (interaction._playerCharacter.playerHands.TryEquipItem(_itemInfo))
        {
            //transform.DOPunchScale(transform.localScale * _pickupTweenScale, _pickupTweenDuration, 0, 0);
            PickupRpc();

            if (_despawnsOnPickup)
                DespawnRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    void PickupRpc()
    {
        OnPickup?.Invoke();
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