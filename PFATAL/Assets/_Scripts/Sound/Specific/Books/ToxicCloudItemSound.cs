using UnityEngine;

public class ToxicCloudItemSound : BookSound<Cons_ToxicCloud>
{
    protected override void LinkEvents()
    {
        base.LinkEvents();
        //main.OnStopUsing += () => AudioManager.Instance.PlayOneShot(Sounds.SmokeLaunch);
    }

    protected override void EquipLink()
    {
        base.EquipLink();
        main.hand.animatorEventListener.OnSpellCast += PlaySound;
    }

    protected override void UnEquipLink()
    {
        base.UnEquipLink();
        main.hand.animatorEventListener.OnSpellCast -= PlaySound;
    }

    void PlaySound() => AudioManager.Instance.PlayOneShot(Sounds.SmokeLaunch);

}
