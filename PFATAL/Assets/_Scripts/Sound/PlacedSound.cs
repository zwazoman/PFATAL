using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlacedSound : MonoBehaviour
{
    [SerializeField] Sounds _sound;
    [SerializeField] bool _moving = true;
    [SerializeField] bool _playOnStart = true;

    EventInstance _currentInstance;

    private void Start()
    {
        if (_playOnStart)
            GameManager.Instance.EventOnGameStarted += PlaySound;
    }

    public void PlaySound()
    {
        print("joue là");

        if (!AudioManager.Instance.playSounds)
            return;

        _currentInstance = AudioManager.Instance.CreateInstance(_sound, true);

        _currentInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        if (_moving)
            RuntimeManager.AttachInstanceToGameObject(_currentInstance, gameObject);


        AudioManager.Instance.Invoke3DInstancePlayed(_currentInstance);
        _currentInstance.start();
    }

    public void StopSound()
    {
        _currentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _currentInstance.release();
    }
}
