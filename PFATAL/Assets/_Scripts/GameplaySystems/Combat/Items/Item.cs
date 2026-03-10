using _scripts.PlayerCharacter;
using UnityEngine;

public class Item : MonoBehaviour
{
    [HideInInspector] protected PlayerCharacter main;
    [HideInInspector] protected Hand carryingHand;

    [SerializeField] GameObject _pickup;

    protected bool isUsing;

    /// <summary>
    /// appelé lorsque le joueur commence l'input d'action de l'item
    /// </summary>
    public virtual void StartUsing()
    {
        isUsing = true;
        Use();
    }

    /// <summary>
    /// appelé toute les frames tant que le joueur garde la touche d'action de l'item enfoncée
    /// </summary>
    public virtual void UseUpdate() { }

    /// <summary>
    /// appelé lorsque le joueur relache la touche d'action de l'item
    /// </summary>
    public virtual void StopUsing()
    {
        isUsing = false;
    }

    /// <summary>
    /// spawn le pickup lié a l'item pour le jeter par terre
    /// </summary>
    public void OnDrop()
    {
        Debug.Log(name + "Dropped !");

        if(_pickup == null)
        {
            Debug.Log("pickup null NATHAN");
            return;
        }

        Vector3 spawnPos = main.playerCamera.transform.position + main.playerCamera.transform.forward * 2;
        Quaternion spawnRot = main.playerCamera.transform.rotation;

        Summoner.Instance.SpawnObject(_pickup, spawnPos, spawnRot);
    }

    public virtual void OnPickup(PlayerCharacter main, Hand hand)
    {
        Debug.Log(name + "Picked up !");

        this.main = main;
        carryingHand = hand;
    }

    public virtual void OnEquip() { }

    public virtual void OnUnEquip() { }

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