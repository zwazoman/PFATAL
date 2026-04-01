using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private bool _isUsingController;
    private bool _changedControls;

    private void Start()
    {
        /*if (Input.GetJoystickNames().Length > 0)
        {
            GetControllerType();
        }*/
        // GetJoystickNames only get the Gamepad/Joystick that are currently active in unity, so i can't get  all the gamepads names from unity registery.
    }

    private void Update()
    {
        if (_isUsingController != true && _changedControls == false)
        {

            _changedControls = true;
        }
    }

    private void SetControlsToGamepad()
    {

    }

    private void SetUIToGamepad()
    {

    }

    public string GetControllerType()
    {
        string controllerName = Input.GetJoystickNames().First();
        Debug.Log(controllerName);
        if (controllerName.ToLower().Contains("xbox"))
        {
            Debug.Log("Hello xbox");
            return "Xbox";
        }
        else if (controllerName.ToLower().Contains("playstation"))
        {
            Debug.Log("Hello playstation");
            return "PlayStation";
        }
        else if (controllerName.ToLower().Contains("pro"))
        {
            Debug.Log("Hello switch");
            return "Switch";
        }
        else
        {
            Debug.Log("Who the f*ck are you?");
            return "Others";
        }
    }
}
