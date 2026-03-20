using UnityEngine;

namespace Settings
{
    public class MouseSensitivitySetter : MonoBehaviour
    {
        [SerializeField] private CharacterAiming _aiming;

        public void AimingSet(float sensitivity)
        {
            _aiming.Sensitivity = sensitivity;
        }
    }
}

