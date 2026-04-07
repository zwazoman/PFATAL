using _scripts.PlayerCharacter;
using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public event Action OnStartUsing;
    public event Action OnUseUpdate;
    public event Action OnStopUsing;

    public event Action OnEquip;
    public event Action OnUnEquip;

    public event Action OnDrop;
    public event Action OnPickup;

    [HideInInspector] public PlayerCharacter playerCharacter;
    [HideInInspector] protected Hand hand;

    [SerializeField] GameObject _pickup;

    protected bool isUsing;
    protected float holdDuration;

    /// <summary>
    /// appel� lorsque le joueur commence l'input d'action de l'item
    /// </summary>
    public virtual void StartUsing()
    {
        OnStartUsing?.Invoke();

        isUsing = true;
        Use();
    }

    /// <summary>
    /// appel� toute les frames tant que le joueur garde la touche d'action de l'item enfonc�e
    /// </summary>
    public virtual void UseUpdate()
    {
        OnUseUpdate?.Invoke();

        holdDuration += Time.deltaTime;
    }

    /// <summary>
    /// appel� lorsque le joueur relache la touche d'action de l'item
    /// </summary>
    public virtual void StopUsing()
    {
        OnStopUsing?.Invoke();

        isUsing = false;
        holdDuration = 0;
    }

    /// <summary>
    /// spawn le pickup li� a l'item pour le jeter par terre
    /// </summary>
    public void Drop()
    {
        OnDrop?.Invoke();

        if(_pickup == null)
        {
            Debug.Log("pickup null NATHAN");
            return;
        }

        Vector3 spawnPos = playerCharacter.playerCamera.transform.position + playerCharacter.playerCamera.transform.forward * 2;
        Quaternion spawnRot = playerCharacter.playerCamera.transform.rotation;

        Summoner.Instance.SpawnObject(_pickup, spawnPos, spawnRot, false);
    }

    public virtual void Pickup(PlayerCharacter main, Hand hand)
    {
        OnPickup?.Invoke();

        playerCharacter = main;
        this.hand = hand;
    }

    public virtual void Equip() { OnEquip?.Invoke(); }

    public virtual void UnEquip()
    {
        OnUnEquip?.Invoke();

        isUsing = false;
        holdDuration = 0;
    }

    async void Use()
    {
        while (isUsing)
        {
            UseUpdate();
            await Awaitable.NextFrameAsync();
        }
    }
}

public enum ItemType
{
    Weapon,
    Consummable
}