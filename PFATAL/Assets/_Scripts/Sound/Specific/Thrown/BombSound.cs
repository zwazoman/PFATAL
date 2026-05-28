using UnityEngine;

public class BombSound : MovingSoundComponent<Cons_Bomb>
{
    protected override void LinkEvents()
    {
        main.OnStartUsing += () => StartSound(Sounds.Fuse3D);


        main.OnEquip += EquipLink;

        main.OnUnEquip += UnEquipLink;
    }

    void EquipLink() => main.hand.animatorEventListener.OnObjectThrown += PlaySound;

    void UnEquipLink() 
    {
        main.hand.animatorEventListener.OnObjectThrown -= PlaySound;
        StopSound();
    } 

    void PlaySound() => AudioManager.Instance.PlayOnlineOneShots(Sounds.TomahawkShoot, Sounds.TomahawkShoot3D, transform.position);
}
