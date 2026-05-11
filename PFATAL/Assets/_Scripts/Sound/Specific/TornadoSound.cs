using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;

public class TornadoSound : MovingSoundComponent<Proj_Tornado>
{
    protected override async void LinkEvents()
    {
        main.OnSpawn += () => StartSound(Sounds.TornadoLaunch3D);
        main.OnDespawn += StopSound;
    }
}
