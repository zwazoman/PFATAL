using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FullScreen : MonoBehaviour
{
    [SerializeField] private Toggle _screenToggle;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;

    private void Awake()
    {
        if (Screen.fullScreen == true)
        {
            _screenToggle.isOn = true;
            _resolutionDropdown.interactable = false;
        }
        else
        {
            _screenToggle.isOn = false;
            _resolutionDropdown.interactable = true;
        }
    }

    public void SetToFull()
    {
        if (Screen.fullScreen == true)
        {
            Screen.fullScreen = false;
            _screenToggle.isOn = false;
            _resolutionDropdown.interactable = true;
        }
        else
        {
            Screen.fullScreen = true;
            _screenToggle.isOn = true;
            _resolutionDropdown.interactable = false;
        }
    }
}
