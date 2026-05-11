using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;

public class MovingSoundComponent<T> : SoundComponent<T> where T: Component
{
    EventInstance _soundInstance;

    protected override void LinkEvents()
    {
    }

    protected virtual void StartSound(Sounds sound)
    {
        _soundInstance = AudioManager.Instance.CreateInstance(sound, true);
        RuntimeManager.AttachInstanceToGameObject(_soundInstance, gameObject);
        _soundInstance.start();
        AudioManager.Instance.Trigger3dSoundPlayed(_soundInstance);
    }

    protected virtual void StopSound()
    {
        _soundInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _soundInstance.release();
    }
}
