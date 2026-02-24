using _scripts.PlayerCharacter;
using UnityEngine;

public class ItemScriptable : ScriptableObject
{
    [HideInInspector] protected PlayerCharacter main;

    [SerializeField] public Mesh mesh;
    [SerializeField] public ItemType type;

    [SerializeField] GameObject _pickup;

    protected bool isUsing;

    public virtual void StartUsing()
    {
        Debug.Log(name + " : Start Using");

        isUsing = true;
        Use();
    }

    public virtual void UseUpdate() { }

    public virtual void StopUsing()
    {
        Debug.Log(name + " : Stop Using");

        isUsing = false;
    }

    public virtual void Break() { }

    public virtual void OnDrop()
    {
        Debug.Log(name + "Dropped !");

        if(_pickup == null)
        {
            Debug.Log("pickup null NATHAN");
            return;
        }

        SpawnContext context = new();

        Vector3 spawnPos = main.playerCamera.transform.position + main.playerCamera.transform.forward * 2;
        Quaternion spawnRot = main.playerCamera.transform.rotation;

        Summoner.Instance.SpawnObject(_pickup, spawnPos, spawnRot, context);
    }

    public virtual void OnPickup(ref PlayerCharacter mainRef)
    {
        Debug.Log(name + "Picked up !");

        main = mainRef;
    }

    public virtual void OnEquip()
    {

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
