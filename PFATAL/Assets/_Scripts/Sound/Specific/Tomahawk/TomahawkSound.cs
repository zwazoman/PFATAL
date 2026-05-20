using UnityEngine;

[RequireComponent(typeof(Tomahawk))]
public class TomahawkSound : ItemSound<Tomahawk>
{
    override protected void LinkEvents()
    {
        base.LinkEvents();
        main.OnShoot += Shoot_Callback;
        main.OnStartGrapple += PlayPreGrappleSound;
    }

    protected override void EquipLink()
    {
        base.EquipLink();
        main.hand.animatorEventListener.OnGrapplePulled += PlayGrappleSound;
    }

    protected override void UnEquipLink()
    {
        base.UnEquipLink();
        main.hand.animatorEventListener.OnGrapplePulled -= PlayGrappleSound;
    }

    void Shoot_Callback() => AudioManager.Instance.PlayOnlineOneShots(Sounds.TomahawkShoot, Sounds.TomahawkShoot3D, transform.position);

    void PlayPreGrappleSound() => AudioManager.Instance.PlayOneShot(Sounds.TomahawkGrappleStart);

    void PlayGrappleSound() => AudioManager.Instance.PlayOnlineOneShots(Sounds.TomahawkGrapple, Sounds.TomahawkGrapple3D, transform.position);


}
