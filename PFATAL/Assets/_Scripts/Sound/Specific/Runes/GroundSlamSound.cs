using FMOD.Studio;
using UnityEngine;

public class GroundSlamSound : RuneSound<Cons_GroundSlam>
{
    protected override void EquipLink()
    {
        base.EquipLink();
        main.hand.animatorEventListener.OnGemBroken += PlayPreSlamDashSound;
    }

    protected override void UnEquipLink()
    {
        base.UnEquipLink();
        main.hand.animatorEventListener.OnGemBroken -= PlayPreSlamDashSound;
    }

    void PlayPreSlamDashSound()
    {
        EventInstance instance = AudioManager.Instance.CreateInstance(Sounds.Jump,true);
        instance.setPitch(-3);
        instance.setVolume(5);
        instance.start();
        instance.release();
    }
}
