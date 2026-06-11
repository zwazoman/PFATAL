using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] SettingsValues _values;

    [Header("Sliders")]
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

        _sfxBus.getVolume(out float oui);
        _musicBus.getVolume(out float non);
        _masterBus.getVolume(out float ptet);
        print($"{oui} {non} {ptet}");

        SetBusVolume(_masterBus, _values.MasterVolume);
        SetBusVolume(_musicBus, _values.MusicVolume);
        SetBusVolume(_sfxBus, _values.SfxVolume);

        _masterSlider.value = _values.MasterVolume;
        _musicSlider.value = _values.MusicVolume;
        _sfxSlider.value = _values.SfxVolume;

        _masterSlider.onValueChanged.AddListener((float value) => SetBusVolume(_masterBus,value));
        _musicSlider.onValueChanged.AddListener((float value) => SetBusVolume(_musicBus,value));
        _sfxSlider.onValueChanged.AddListener((float value) => SetBusVolume(_sfxBus,value));
    }

    void SetBusVolume(Bus bus, float newValue)
    {
        print(newValue);
        bus.setVolume(newValue);
    }
}
