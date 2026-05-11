using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private PlayerCharacterInputs _input;
    private bool _setControllerType = false;
    private string _wichType;

    [Header("Change Settings for Controller")]
    [SerializeField] private List<GameObject> _PCMovementsButtons;

    [SerializeField] private RawImage ImageCurrentController;
    [SerializeField] private List<Texture2D> GeneralControllerUI;

    [Header("UI Rebinds")]
    public List<Texture2D> CurrentControlsUI;
    [SerializeField] private List<Texture2D> SwitchControllsUI;
    [SerializeField] private List<Texture2D> PlaystationControllsUI;
    [SerializeField] private List<Texture2D> XboxControllsUI;
    [SerializeField] private List<Texture2D> PCControllsUI;


    private void Start()
    {
        //CurrentControlsUI = SwitchControlls;
    }

    private void FixedUpdate()
    {
        if (Gamepad.current == null)
        {
            _wichType = "Computer";
            CurrentControlsUI = PCControllsUI;
        }
        else
        {
            if (_input.UsingGamePad == true)
            {
                if (_setControllerType == true)
                {
                    switch (_wichType)
                    {
                        case "Computer":
                            CurrentControlsUI = PCControllsUI;

                            ChangeUI("keyboard");
                            break;
                        case "Xbox":
                            CurrentControlsUI = XboxControllsUI;

                            ChangeUI("xbox");
                            break;
                        case "PlayStation":
                            CurrentControlsUI = PlaystationControllsUI;

                            ChangeUI("playstation");
                            break;
                        case "Switch":
                            CurrentControlsUI = SwitchControllsUI;

                            ChangeUI("switch");
                            break;
                        case "Others":

                            break;
                    }
                        
                    _setControllerType = false;
                }
            }
        }
    }

    public string GetControllerType()
    {
        _setControllerType = true;
        string controllerName = Input.GetJoystickNames().First();
        Debug.Log(controllerName);
        if (controllerName.ToLower().Contains("xbox"))
        {
            Debug.Log("Hello xbox");
            return _wichType = "Xbox";
        }
        else if (controllerName.ToLower().Contains("playstation"))
        {
            Debug.Log("Hello playstation");
            return _wichType = "PlayStation";
        }
        else if (controllerName.ToLower().Contains("pro"))
        {
            Debug.Log("Hello switch");
            return _wichType = "Switch";
        }
        else
        {
            Debug.Log("Who the f*ck are you?");
            return _wichType = "Others";
        }
    }

    private void ChangeUI(string controller)
    {
        int i = 0;

        foreach (Texture2D texture in GeneralControllerUI)
        {
            if (_PCMovementsButtons.Count < i)
            {
                _PCMovementsButtons[i].SetActive(false);
            }
            if (texture.name.Contains(controller))
            {
                ImageCurrentController.texture = GeneralControllerUI[i];
            }
            i++;
        }
    }
}
