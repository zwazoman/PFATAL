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

    [SerializeField] private Button _jumpRebind;
    [SerializeField] private Button _pickupRebind;
    [SerializeField] private Button _dropRebind;
    [SerializeField] private Slider _cameraSensitivity;
    [SerializeField] private List<GameObject> _controllerUINav;
    private Navigation _oldNavPickup = new Navigation();
    private Navigation _oldNavJump = new Navigation();
    private Navigation _oldNavDrop = new Navigation();
    private Navigation _oldNavSensitivity = new Navigation();
    private Navigation _newNavPickup = new Navigation();
    private Navigation _newNavJump = new Navigation();
    private Navigation _newNavDrop = new Navigation();
    private Navigation _newNavSensitivity = new Navigation();
    private Vector3 _oldXPosJump;
    private Vector3 _oldXPosPickup;
    private Vector3 _oldXPosDrop;
    private Vector3 _newXPosJump;
    private Vector3 _newXPosPickup;
    private Vector3 _newXPosDrop;


    private void Start()
    {
        //CurrentControlsUI = SwitchControlls;

        _oldNavPickup.mode = Navigation.Mode.Explicit;
        _oldNavJump.mode = Navigation.Mode.Explicit;
        _oldNavDrop.mode = Navigation.Mode.Explicit;
        _oldNavSensitivity.mode = Navigation.Mode.Explicit;
        _oldNavPickup = _pickupRebind.navigation;
        _oldNavJump = _jumpRebind.navigation;
        _oldNavDrop = _dropRebind.navigation;
        _oldNavSensitivity = _cameraSensitivity.navigation;

        _newNavPickup.mode = Navigation.Mode.Explicit;
        _newNavJump.mode = Navigation.Mode.Explicit;
        _newNavDrop.mode = Navigation.Mode.Explicit;
        _newNavSensitivity.mode = Navigation.Mode.Explicit;

        _newNavPickup.selectOnUp = _controllerUINav[1].GetComponent<Button>();
        _newNavPickup.selectOnDown = _controllerUINav[3].GetComponent<Button>();

        _newNavJump.selectOnUp = _controllerUINav[0].GetComponent<Slider>();
        _newNavJump.selectOnDown = _controllerUINav[2].GetComponent<Button>();

        _newNavDrop.selectOnUp = _controllerUINav[2].GetComponent<Button>();

        _newNavSensitivity.selectOnUp = _controllerUINav[4].GetComponent<Slider>();
        _newNavSensitivity.selectOnDown = _controllerUINav[1].GetComponent<Button>();

        _oldXPosJump = _controllerUINav[1].transform.position;
        _oldXPosPickup = _controllerUINav[2].transform.position;
        _oldXPosDrop = _controllerUINav[3].transform.position;

        _newXPosJump = _controllerUINav[1].transform.position;
        _newXPosPickup = _controllerUINav[2].transform.position;
        _newXPosDrop = _controllerUINav[3].transform.position;
        _newXPosJump.x = 1125f;
        _newXPosPickup.x = 1125f;
        _newXPosDrop.x = 1125;
    }

    private void FixedUpdate()
    {
        if (Gamepad.current == null)
        {
            _wichType = "Computer";
            CurrentControlsUI = PCControllsUI;

            _pickupRebind.navigation = _oldNavPickup;
            _jumpRebind.navigation = _oldNavJump;
            _dropRebind.navigation = _oldNavDrop;
            _cameraSensitivity.navigation = _oldNavSensitivity;

            _controllerUINav[1].transform.position = _oldXPosJump;
            _controllerUINav[2].transform.position = _oldXPosPickup;
            _controllerUINav[3].transform.position = _oldXPosDrop;

            ChangeUI("keyboard");
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
                            
                            break;
                        case "Xbox":
                            CurrentControlsUI = XboxControllsUI;

                            _pickupRebind.navigation = _newNavPickup;
                            _jumpRebind.navigation = _newNavJump;
                            _dropRebind.navigation = _newNavDrop;
                            _cameraSensitivity.navigation = _newNavSensitivity;

                            _controllerUINav[1].transform.position = _newXPosJump;
                            _controllerUINav[2].transform.position = _newXPosPickup;
                            _controllerUINav[3].transform.position = _newXPosDrop;

                            ChangeUI("xbox");
                            break;
                        case "PlayStation":
                            CurrentControlsUI = PlaystationControllsUI;

                            _pickupRebind.navigation = _newNavPickup;
                            _jumpRebind.navigation = _newNavJump;
                            _dropRebind.navigation = _newNavDrop;
                            _cameraSensitivity.navigation = _newNavSensitivity;

                            _controllerUINav[1].transform.position = _newXPosJump;
                            _controllerUINav[2].transform.position = _newXPosPickup;
                            _controllerUINav[3].transform.position = _newXPosDrop;

                            ChangeUI("playstation");
                            break;
                        case "Switch":
                            CurrentControlsUI = SwitchControllsUI;

                            _pickupRebind.navigation = _newNavPickup;
                            _jumpRebind.navigation = _newNavJump;
                            _dropRebind.navigation = _newNavDrop;
                            _cameraSensitivity.navigation = _newNavSensitivity;

                            _controllerUINav[1].transform.position = _newXPosJump;
                            _controllerUINav[2].transform.position = _newXPosPickup;
                            _controllerUINav[3].transform.position = _newXPosDrop;

                            ChangeUI("switch");
                            break;
                        case "Others":

                            break;
                    }
                        
                    _setControllerType = false;
                }
            }
            else
            {
                CurrentControlsUI = PCControllsUI;

                _pickupRebind.navigation = _oldNavPickup;
                _jumpRebind.navigation = _oldNavJump;
                _dropRebind.navigation = _oldNavDrop;
                _cameraSensitivity.navigation = _oldNavSensitivity;

                _controllerUINav[1].transform.position = _oldXPosJump;
                _controllerUINav[2].transform.position = _oldXPosPickup;
                _controllerUINav[3].transform.position = _oldXPosDrop;

                ChangeUI("keyboard");
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
            if (_PCMovementsButtons.Count > i && controller != "keyboard")
            {
                _PCMovementsButtons[i].SetActive(false);
            }
            if (_PCMovementsButtons.Count > i && controller == "keyboard")
            {
                _PCMovementsButtons[i].SetActive(true);
            }
            if (texture.name.Contains(controller))
            {
                ImageCurrentController.texture = GeneralControllerUI[i];
            }
            i++;
        }
    }
}
