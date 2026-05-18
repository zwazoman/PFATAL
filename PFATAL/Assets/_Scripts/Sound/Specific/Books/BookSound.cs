using UnityEngine;

public class BookSound<T> : ItemSound<T> where T : Cons_BookBase
{
    protected override void LinkEvents()
    {
        
    }

    protected override void EquipLink()
    {
        //main.hand.animatorEventListener.OnSpellCast += le son
    }
}
