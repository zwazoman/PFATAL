using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _sfxSlider;

    Bus _masterBus;
    Bus _musicBus;
    Bus _sfxBus;

    private void Start()
    {
        _sfxBus = RuntimeManager.GetBus("bus:/GlobalSFX");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _masterBus = RuntimeManager.GetBus("bus:/");

        float masterVolume;
        float musicVolume;
        float sfxVolume;

        _sfxBus.setVolume(.3f);

        _masterBus.getVolume(out masterVolume);
        _musicBus.getVolume(out musicVolume);
        _sfxBus.getVolume(out sfxVolume);

        print($"{masterVolume} {musicVolume} {sfxVolume}");

        return;

        _masterSlider.value = masterVolume;
        _musicSlider.value = musicVolume;
        _sfxSlider.value = sfxVolume;
    }

    public void SetMusicVolume()
    {
        
    }

    void SetBusVolume(Bus bus, float newValue)
    {
        bus.setVolume(newValue);
    }
}
