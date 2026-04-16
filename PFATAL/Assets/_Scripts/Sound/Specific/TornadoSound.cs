using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;

public class TornadoSound : SoundComponent<Proj_Tornado>
{
    EventInstance _tornadoInstance;

    protected override void LinkEvents()
    {
        main.OnSpawn += StartSound;
        main.OnDespawn += StopSound;
    }

    void StartSound()
    {
        _tornadoInstance = AudioManager.Instance.CreateInstance(Sounds.TornadoLaunch3D, true);
        RuntimeManager.AttachInstanceToGameObject(_tornadoInstance, gameObject, true);
        _tornadoInstance.start();
    }

    void StopSound()
    {
        _tornadoInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _tornadoInstance.release();

        _tornadoInstance = default;
    }

}
