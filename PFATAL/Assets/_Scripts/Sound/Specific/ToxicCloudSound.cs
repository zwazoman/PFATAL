using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class ToxicCloudSound : SoundComponent<ToxicCloud>
{
    EventInstance _smokeInstance;

    protected override void LinkEvents()
    {
        print("link");

        main.OnSmokeStart += StartSound;
        main.OnSmokeEnd += StopSound;
    }

    public void StartSound()
    {
        print("smoke sound");

        _smokeInstance = AudioManager.Instance.CreateInstance(Sounds.Smoke3D, true);
        _smokeInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        _smokeInstance.start();
    }

    void StopSound()
    {
        print("no smoke sound");

        _smokeInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _smokeInstance.release();

        _smokeInstance = default;
    }

}
