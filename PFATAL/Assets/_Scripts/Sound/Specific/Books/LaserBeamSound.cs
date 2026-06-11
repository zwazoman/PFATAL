using UnityEngine;

public class LaserBeamSound : BookSound<Cons_BigLaserBeam>
{
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

    void PlaySound()
    {
        print("spell cast");
        AudioManager.Instance.PlayOnlineOneShots(Sounds.LaserBeam, Sounds.LaserBeam3D, transform.position);
    }
}
