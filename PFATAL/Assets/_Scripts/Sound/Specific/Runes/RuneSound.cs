using UnityEngine;

public class RuneSound<T> : ItemSound<T> where T : Cons_RuneBase
{
    protected override void EquipLink() => main.hand.animatorEventListener.OnGemBroken += UseSound;

    protected override void UnEquipLink() => main.hand.animatorEventListener.OnGemBroken -= UseSound;

    protected override void UseSound() => AudioManager.Instance.PlayOneShot(Sounds.BreakRune);
}
