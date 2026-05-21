using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using _scripts.PlayerCharacter;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.Netcode.Editor.Configuration;

namespace Settings
{
    public class Rebinds : MonoBehaviour
    {
        [Header("Rebinds")]
        public InputActionAsset InputActions;
        public InputActionReference InputReference;
        [SerializeField] private EventSystem _system;
        [SerializeField] private GameObject _controlsButton;
        [SerializeField] private GameObject _menu;
        [SerializeField] private Button _returnButton;
        [SerializeField] private GameObject _errorMessage;

        [Header("Reset Menu")]
        [SerializeField] private List<GameObject> _menuMainWindows;

        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

        private InputAction _moveAction;
        private TMP_Text _rebindLabel;

        private int _bindingIndex;

        [Header("Controller changes")]
        [SerializeField] private PlayerCharacter _playerCharacter;
        [SerializeField] private InputHandler _inputHandler;
        private Gamepad _gamepad;

        private void Awake()
        {
            //_moveAction = InputActions.FindAction("Move");
            _moveAction = InputReference;
        }

        public void OpenClose(InputAction.CallbackContext context)
        {
            if (Input.GetJoystickNames().Length > 0)
            {
                _gamepad = Gamepad.current;
                //For testing and to know what type of controller we have.
                if (context.action.activeControl.device.name == _gamepad.name)
                {
                    _system.SetSelectedGameObject(_controlsButton);
                    _inputHandler.GetControllerType();
                }
            }

            if (context.performed && _menu.activeInHierarchy == true)
            {
                CloseAndSwapAction();
            }
            else if (context.performed && _menu.activeInHierarchy == false)
            {
                _menu.SetActive(true);
                _playerCharacter.SwapActionMapToUI();
                if (_playerCharacter.inputs.UsingGamePad == true)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        public void CloseAndSwapAction()
        {
            for (int i = 0; i < _menuMainWindows.Count; i++)
            {
                if (i == 0)
                {
                    _menuMainWindows[0].SetActive(true);
                }
                else
                {
                    _menuMainWindows[i].SetActive(false);
                }
            }
            _menu.SetActive(false);
            _playerCharacter.SwapActionMapToPlayer();
            Cursor.lockState = CursorLockMode.Locked;
        }

        //To change later for the script in wich it will do every changes for the UI.
        //Also it's for giving a specific TMP_Text to the buttons in unity.
        public void ChangeRebindText(TMP_Text text)
        {
            _rebindLabel = text;
            text.text = "...";
        }

        //If you want to rebind a specific movement -> put in the string one of those WITHOUT ANY UPPERCASES (PAS DE MAJUSCULES) : up / down / left / right
        //Also, we do not need a seperate one for controllers because we do not need to rebind the joystick of a controller.
        public void RebindMovements(string movementToChange)
        {
            InputActions.FindActionMap("UI").Disable();
            _bindingIndex = InputReference.action.bindings.IndexOf(x => x.isPartOfComposite && x.name == movementToChange);

            _rebindingOperation = InputReference.action.PerformInteractiveRebinding().WithControlsExcluding("<Mouse>").WithControlsExcluding("<Gamepad>").WithTargetBinding(_bindingIndex).OnMatchWaitForAnother(.2f).OnComplete(
                operation =>
                {
                    if (CheckupDuplicatesBinding(_moveAction, _bindingIndex, true) == true)
                    {
                        _moveAction.RemoveBindingOverride(_bindingIndex);
                        _rebindLabel.text = "";
                        _rebindingOperation.Dispose();
                        InputActions.FindActionMap("UI").Enable();
                        return;
                    }
                    RebindCompleted(_moveAction);
                }).Start();
            //RebindCompleted(_moveAction)).Start();
        }

        public void RebindSingleInput(InputActionReference action)
        {
            InputActions.FindActionMap("UI").Disable();

            if (_playerCharacter.inputs.UsingGamePad == true)
            {
                _rebindingOperation = action.action.PerformInteractiveRebinding(1).
                    WithControlsExcluding("<Mouse>").
                    WithControlsExcluding("<Keyboard>").
                    WithControlsExcluding("<Gamepad>/leftstick").
                    WithControlsExcluding("<Gamepad>/leftstickpress").
                    WithControlsExcluding("<Gamepad>/rightstickpress").
                    WithControlsExcluding("<Gamepad>/rightstick").
                    //WithControlsExcluding("<Gamepad>/leftshoulder"). //To be determined if i unlock them or not.
                    //WithControlsExcluding("<Gamepad>/rightshoulder"). //To be determined if i unlock them or not.
                    WithControlsExcluding("<Gamepad>/lefttrigger").
                    WithControlsExcluding("<Gamepad>/righttrigger").
                    WithControlsExcluding("<Gamepad>/dpad"). //To be determined if i unlock them or not.
                    WithControlsExcluding("<Gamepad>/start").
                    WithControlsExcluding("<Gamepad>/select").
                    WithControlsExcluding("<Gamepad>/capture").
                    WithControlsExcluding("<Gamepad>/home").
                    WithTargetBinding(0).OnMatchWaitForAnother(.2f).OnComplete(
                operation =>
                {
                    if (CheckupDuplicatesBinding(action, 0, false) == true)
                    {
                        action.action.RemoveBindingOverride(0);
                        _rebindLabel.text = "";
                        _rebindingOperation.Dispose();
                        InputActions.FindActionMap("UI").Enable();
                        return;
                    }
                    _bindingIndex = 0;
                    RebindCompleted(action);
                }).Start();
            }
            else
            {
                _rebindingOperation = action.action.PerformInteractiveRebinding(0).WithControlsExcluding("<Mouse>").WithControlsExcluding("<Gamepad>").WithTargetBinding(0).OnMatchWaitForAnother(.2f).OnComplete(
                operation =>
                {
                    if (CheckupDuplicatesBinding(action, 0, false) == true)
                    {
                        action.action.RemoveBindingOverride(0);
                        _rebindLabel.text = "";
                        _rebindingOperation.Dispose();
                        InputActions.FindActionMap("UI").Enable();
                        return;
                    }
                    _bindingIndex = 0;
                    RebindCompleted(action);
                }).Start();
                //RebindCompleted(action)).Start();
            }
        }

        private void RebindCompleted(InputAction action)
        {
            _rebindingOperation.Dispose();
            Debug.Log(_bindingIndex);
            //string newBinding = InputControlPath.ToHumanReadableString(action.bindings[_bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            string newBinding = action.GetBindingDisplayString(_bindingIndex).ToUpper();
            Debug.Log(newBinding);
            //To change later for the script in wich it will do every changes for the UI.
            _rebindLabel.text = $"{newBinding}";
            InputActions.FindActionMap("UI").Enable();
        }

        private bool CheckupDuplicatesBinding(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            InputBinding newBinding = action.bindings[bindingIndex];

            int currentIndex = -1;

            foreach (InputBinding bindings in action.actionMap.bindings)
            {
                currentIndex++;

                if (bindings.action == newBinding.action)
                {
                    if (bindings.isPartOfComposite && currentIndex != bindingIndex)
                    {
                        if (newBinding.effectivePath == bindings.effectivePath)
                        {
                            _returnButton.interactable = false;
                            _errorMessage.SetActive(true);
                            return true;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                if (newBinding.effectivePath == bindings.effectivePath)
                {
                    _returnButton.interactable = false;
                    _errorMessage.SetActive(true);
                    return true;
                }
            }

            if (allCompositeParts == true)
            {
                for (int i = 0; i < bindingIndex; i++)
                {
                    if (action.bindings[i].effectivePath == newBinding.overridePath)
                    {
                        _returnButton.interactable = false;
                        _errorMessage.SetActive(true);
                        return true;
                    }
                }
            }
            _errorMessage.SetActive(false);
            _returnButton.interactable = true;
            return false;
        }
    }
}

