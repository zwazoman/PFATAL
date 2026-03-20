using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Settings
{
    public class MaximizedScreenSetter : MonoBehaviour
    {
        [SerializeField] private Toggle _MaximizedToggle;
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private Toggle _fullscreenToggle;

        private void Awake()
        {
            /*
            if (Screen.currentResolution ==)
            {

            }*/
        }

        public void SetToMaximized(bool onOrNot)
        {
            if (onOrNot == true)
            {
                _MaximizedToggle.isOn = true;
                Screen.SetResolution(1920, 1080, FullScreenMode.MaximizedWindow);
                _resolutionDropdown.interactable = false;
                _fullscreenToggle.interactable = false;
            }
            else
            {
                _MaximizedToggle.isOn = false;
                Screen.SetResolution(1920, 1080, false);
                _resolutionDropdown.interactable = true;
                _fullscreenToggle.interactable = true;
                _fullscreenToggle.isOn = false;
            }
        }
    }
}

