using UnityEngine;

namespace Settings
{
    public class MouseSensitivitySetter : MonoBehaviour
    {
        [SerializeField] private CharacterAiming _aiming;
        [SerializeField] private SettingsValues _values;

        public void AimingSet(float sensitivity)
        {
            _values.SensitivityMouse = sensitivity;

            if (_aiming == null) return;
            _aiming.Sensitivity = sensitivity;
        }

        public void ControllerAImingSet(float sensitivity)
        {
            _values.SensitivityController = sensitivity;

            if (_aiming == null) return;
            _aiming.ControllerSensitivity = sensitivity;
        }
    }
}

