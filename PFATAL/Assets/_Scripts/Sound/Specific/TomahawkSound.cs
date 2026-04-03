using UnityEngine;

[RequireComponent(typeof(Tomahawk))]
public class TomahawkSound : SoundComponent<Tomahawk>
{
    override protected void LinkEvents()
    {
        main.OnShoot += Shoot_Callback;
    }

    void Shoot_Callback()
    {
        AudioManager.Instance.PlayOnlineOneShots(Sounds.TomahawkShoot, Sounds.TomahawkShoot3D, transform.position/*, main.playerCharacter.OwnerClientId*/);
    }
}
