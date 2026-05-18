using UnityEngine;

public class HealSound : RuneSound<Cons_Heal>
{
    protected override void EquipLink()
    {
        base.EquipLink();
        main.hand.animatorEventListener.OnGemBroken += PlayHealSound;
    }

    protected override void UnEquipLink()
    {
        base.UnEquipLink();
        main.hand.animatorEventListener.OnGemBroken -= PlayHealSound;
    }

    void PlayHealSound() => AudioManager.Instance.PlayOneShot(Sounds.Heal);
}
