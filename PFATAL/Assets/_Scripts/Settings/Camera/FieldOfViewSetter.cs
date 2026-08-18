using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class FieldOfViewSetter : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] Slider _fovSlider;
        private void Start()
        {
            _fovSlider.onValueChanged.AddListener(setFOV);
            _fovSlider.value = PlayerPrefs.GetFloat("FOV");

        }

        // I can't fucking do it because SOMEONE made it so that the FOV will ALWAYS STAY THE SAME INSIDE OF A LATE UPDATE!
        // (It's in PlayerCameraBehaviour for reference).
        void setFOV(float sliderAmount)
        {
            PlayerCameraBehaviour.BaseFov = sliderAmount;
            PlayerPrefs.SetFloat("FOV", sliderAmount);
        }
    }
}

