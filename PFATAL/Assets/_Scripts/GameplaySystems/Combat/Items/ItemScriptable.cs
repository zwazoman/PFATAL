using _scripts.PlayerCharacter;
using UnityEngine;

[CreateAssetMenu(fileName = "newItem",menuName = "Item")]
public class ItemScriptable : ScriptableObject
{
    [HideInInspector] protected PlayerCharacter main;

    [SerializeField] public Mesh mesh;
    [SerializeField] public ItemType type;

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
