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
    public Hand hand { get; private set; }
    public bool isUsing { get; private set; }
    public bool canScrolling { get; private set; } = true;


    [SerializeField] GameObject _pickup;

    protected float holdDuration;

    public int ItemID => this switch
    {
        Sword => 0,
        Tomahawk => 1,
        Crossbow => 2,
        Cons_Tornado => 3,
        Cons_WolfTrap => 4,
        Cons_Bomb => 5,
        Cons_BigLaserBeam => 6,
        Cons_Heal => 7,
        Cons_TP => 8,
        Cons_GroundSlam => 9,
        Cons_ToxicCloud => 10,

        _ => -1
    };

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
    /// appel� lorsque le joueur relache la touche d'action de l'item si le joueur a d'abord "startusing" l'item
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
        playerCharacter = main;
        this.hand = hand;

        OnPickup?.Invoke();
    }

    /// <summary>
    /// appelé quand la main equipe l'item
    /// </summary>
    public virtual void Equip() { OnEquip?.Invoke(); }

    public virtual void UnEquip()
    {
        OnUnEquip?.Invoke();

        isUsing = false;
        holdDuration = 0;
    }

    public virtual void Scrollable()
    {
        canScrolling = true;
        print("[Item] scrollable");
    }

    public virtual void StopScrollable()
    {
        canScrolling = false;
        print("[Item] not scrollable");
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