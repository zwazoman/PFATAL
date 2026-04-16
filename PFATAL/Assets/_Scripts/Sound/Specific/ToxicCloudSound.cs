using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ToxicCloudSound : SoundComponent<ToxicCloud>
{
    EventInstance _smokeInstance;

    protected override void LinkEvents()
    {
        main.OnSmokeStart += StartSound;
        main.OnSmokeEnd += StopSound;
    }

    void StartSound()
    {
        _smokeInstance = AudioManager.Instance.CreateInstance(Sounds.Smoke, true);
        _smokeInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        _smokeInstance.start();
    }

    void StopSound()
    {
        _smokeInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _smokeInstance.release();
    }

}
