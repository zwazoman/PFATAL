using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class MouseSensitivitySetter : MonoBehaviour
    {
        [SerializeField] private CharacterAiming _aiming;
        [SerializeField] Slider _sensSlider;

        private void Start()
        {
            _sensSlider.onValueChanged.AddListener(SetSens);
            _sensSlider.value = PlayerPrefs.GetFloat("Sensitivity", 17.5f);
        }

        void SetSens(float sens)
        {
            PlayerPrefs.SetFloat("Sensitivity", sens);

            if (_aiming == null) return;
            _aiming.Sensitivity = sens;
            _aiming.ControllerSensitivity = sens; ;
        }
    }
}

