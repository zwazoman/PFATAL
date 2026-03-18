using UnityEngine;

namespace Settings
{
    public class MouseSensitivitySetter : MonoBehaviour
    {
        [SerializeField] private CharacterAiming _aiming;

        private void AimingSet(float sensitivity)
        {
            //_aiming.Sensitivity = sensitivity;
        }
    }
}

