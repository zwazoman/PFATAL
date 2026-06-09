using UnityEngine;

public class LaserBeamSound : BookSound<Cons_BigLaserBeam>
{
    protected override void EquipLink()
    {
        base.EquipLink();
        main.hand.animatorEventListener.OnSpellCast += PlaySound;
    }

    void PlaySound()
    {
        AudioManager.Instance.PlayOnlineOneShots(Sounds.LaserBeam, Sounds.LaserBeam3D, transform.position);
    }
}
