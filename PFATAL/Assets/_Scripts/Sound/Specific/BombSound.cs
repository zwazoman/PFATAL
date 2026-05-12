using UnityEngine;

public class BombSound : MovingSoundComponent<Cons_Bomb>
{
    protected override void LinkEvents()
    {
        main.OnStartUsing += () => StartSound(Sounds.Fuse3D);
        main.OnStopUsing += StopSound;
        main.OnUnEquip += StopSound;
    }
}
