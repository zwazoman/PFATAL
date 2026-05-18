using UnityEngine;

public class BombProjSound : MovingSoundComponent<Proj_Bomb>
{
    [SerializeField] float _soundDuration = 17.254f;

    protected override void LinkEvents()
    {
        main.OnSpawn += Spawn_Callback;
        main.OnExplode += () => StopSound(false);
    }

    void Spawn_Callback()
    {
        if (main.fuseTimer >= main.fuseTime)
            return;

        float offsetValue = (main.fuseTimer / _soundDuration);

        print(offsetValue);
        StartSound(Sounds.Fuse3D, null, "StartOffset", offsetValue);
    }
}
