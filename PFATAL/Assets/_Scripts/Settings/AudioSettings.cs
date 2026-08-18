using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioSettings : MonoBehaviour
{
    string _masterVolumePref = "MasterVolume";
    string _musicVolumePref = "MusicVolume";
    string _sfxVolumePref = "SFXVolume";

    [Header("Sliders")]
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _sfxSlider;

    Bus _masterBus;
    Bus _musicBus;
    Bus _sfxBus;

    private void Start()
    {
        if (!AudioManager.Instance.playSounds)
            return;

        _sfxBus = RuntimeManager.GetBus("bus:/GlobalSFX");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _masterBus = RuntimeManager.GetBus("bus:/");

        _masterSlider.onValueChanged.AddListener((float value) => SetBusVolume(_masterBus, _masterVolumePref, value));
        _musicSlider.onValueChanged.AddListener((float value) => SetBusVolume(_musicBus, _musicVolumePref, value));
        _sfxSlider.onValueChanged.AddListener((float value) => SetBusVolume(_sfxBus, _sfxVolumePref, value));

        _masterSlider.value = PlayerPrefs.GetFloat(_masterVolumePref, 1);
        _musicSlider.value = PlayerPrefs.GetFloat(_musicVolumePref, 1);
        _sfxSlider.value = PlayerPrefs.GetFloat(_sfxVolumePref, 1);
    }

    void SetBusVolume(Bus bus, string prefName, float newValue)
    {
        bus.setVolume(newValue);
        PlayerPrefs.SetFloat(prefName, newValue);
    }
}
