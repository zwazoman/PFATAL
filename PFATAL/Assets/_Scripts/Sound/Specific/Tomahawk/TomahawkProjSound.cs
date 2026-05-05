using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;

public class TomahawkProjSound : MovingSoundComponent<Visual_Proj_Tomahawk>
{
    protected override void LinkEvents()
    {
        main.OnSpawn += () => StartSound(Sounds.TomahawkSpin3D);
        main.OnDespawn += StopSound;
    }
}
