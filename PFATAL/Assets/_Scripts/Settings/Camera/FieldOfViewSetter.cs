using UnityEngine;

namespace Settings
{
    public class FieldOfViewSetter : MonoBehaviour
    {
        [SerializeField] private SettingsValues _values;
        [SerializeField] private Camera _camera;

        // I can't fucking do it because SOMEONE made it so that the FOV will ALWAYS STAY THE SAME INSIDE OF A LATE UPDATE!
        // (It's in PlayerCameraBehaviour for reference).
        public void FOVSet(float sliderAmount)
        {
            _values.FOV = sliderAmount;
            //if ()
            PlayerCameraBehaviour.BaseFov = sliderAmount;
        }
    }
}

