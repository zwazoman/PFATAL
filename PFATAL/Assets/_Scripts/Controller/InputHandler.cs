using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private PlayerCharacterInputs _input;
    private bool _setControllerType = false;
    private string _wichType;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Gamepad.current == null)
        {
            _wichType = "Computer";
        }
        else
        {
            if (_input.UsingGamePad == true)
            {
                if (_setControllerType == true)
                {
                    //switch
                    //_setControllerType = false;
                }
            }
        }
    }

    public string GetControllerType()
    {
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
}
