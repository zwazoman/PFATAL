using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using UnityEngine;

public abstract class MovingSoundComponent<T> : SoundComponent<T> where T: Component
{
    EventInstance _soundInstance;

    abstract protected override void LinkEvents();

    protected virtual void StartSound(Sounds sound, GameObject go = null, string parameterName = null, float parameterValue = 100)
    {
        GameObject currentGO = null;

        if (go == null)
            currentGO = gameObject;
        else
            currentGO = go;

        _soundInstance = AudioManager.Instance.CreateInstance(sound, true);

        if(parameterName != null && parameterValue !=100)
            _soundInstance.setParameterByName(parameterName, parameterValue);

        RuntimeManager.AttachInstanceToGameObject(_soundInstance, currentGO);
        _soundInstance.start();
        AudioManager.Instance.Trigger3dSoundPlayed(_soundInstance);
    }

    protected virtual void StopSound()
    {
        if (!_soundInstance.isValid())
            return;

        _soundInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _soundInstance.release();
    }
}
