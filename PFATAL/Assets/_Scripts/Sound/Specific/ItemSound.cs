using UnityEngine;

public class ItemSound<T> : SoundComponent<T> where T : Item
{
    protected override void LinkEvents()
    {
        main.OnEquip += EquipLink;
        main.OnUnEquip += UnEquipLink;
    }

    protected virtual void EquipLink() { }

    protected virtual void UnEquipLink() { }

    protected virtual void UseSound() { }
}
