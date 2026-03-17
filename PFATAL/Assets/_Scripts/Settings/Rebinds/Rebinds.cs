using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using _scripts.PlayerCharacter;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Rebinds : MonoBehaviour
{
    public InputActionAsset InputActions;
    public InputActionReference InputReference;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    private InputAction _moveAction;
    private InputAction _action;
    private TMP_Text _rebindLabel;

    private int _bindingIndex;

    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private GameObject _menu;
    
    private void Awake()
    {
        //_moveAction = InputActions.FindAction("Move");
        _moveAction = InputReference;
    }

    //needs to be somewhere else.
    public void OpenClose(InputAction.CallbackContext context)
    {
        //Why does it not work?
        if (context.performed && _menu.activeInHierarchy == true)
        {
            CloseAndSwapAction();
        }
        else
        {
            _menu.SetActive(true);
            _playerCharacter.SwapActionMapToUI();
        }
    }

    public void CloseAndSwapAction()
    {
        _menu.SetActive(false);
        _playerCharacter.SwapActionMapToPlayer();
    }

    //To change later for the script in wich it will do every changes for the UI.
    //Also it's for giving a specific TMP_Text to the buttons in unity.
    public void ChangeRebindText(TMP_Text text)
    {
        _rebindLabel = text;
        text.text = "...";
    }

    // If you want to rebind a specific movement -> put in the string one of those WITHOUT ANY UPPERCASES (PAS DE MAJUSCULES) : up / down / left / right
    public void RebindMovements(string movementToChange)
    {
        InputActions.FindActionMap("Player").Disable();
        _bindingIndex = InputReference.action.bindings.IndexOf(x => x.isPartOfComposite && x.name == movementToChange);

        _rebindingOperation = InputReference.action.PerformInteractiveRebinding().WithControlsExcluding("<Mouse>").WithControlsExcluding("<Gamepad>").WithTargetBinding(_bindingIndex).OnMatchWaitForAnother(.2f).OnComplete(
            operation =>
            {
                if (CheckupDuplicatesBinding(_moveAction, _bindingIndex, true) == true)
                {
                    _moveAction.RemoveBindingOverride(_bindingIndex);
                    _rebindLabel.text = "";
                    _rebindingOperation.Dispose();
                    return;
                }
                RebindCompleted(_moveAction);
            }).Start();
            //RebindCompleted(_moveAction)).Start();
    }

    public void RebindSingleInput(InputActionReference action)
    {
        InputActions.FindActionMap("Player").Disable();

        _rebindingOperation = action.action.PerformInteractiveRebinding(0).WithControlsExcluding("<Mouse>").WithControlsExcluding("<Gamepad>").WithTargetBinding(0).OnMatchWaitForAnother(.2f).OnComplete(
            operation =>
            {
                if (CheckupDuplicatesBinding(action, 0, false) == true)
                {
                    action.action.RemoveBindingOverride(0);
                    _rebindLabel.text = "";
                    _rebindingOperation.Dispose();
                    return;
                }
                _bindingIndex = 0;
                RebindCompleted(action);
            }).Start();
            //RebindCompleted(action)).Start();
    }

    private void RebindCompleted(InputAction action)
    {
        _rebindingOperation.Dispose();
        Debug.Log(_bindingIndex);
        //string newBinding = InputControlPath.ToHumanReadableString(action.bindings[_bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        string newBinding = action.GetBindingDisplayString(_bindingIndex).ToUpper();
        //To change later for the script in wich it will do every changes for the UI.
        _rebindLabel.text = $"{newBinding}";
        //InputActions.FindActionMap("Player").Enable();
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
                    if(newBinding.effectivePath == bindings.effectivePath)
                    {
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
                return true;
            }
        }

        if (allCompositeParts == true)
        {
            for (int i = 0; i < bindingIndex; i++)
            {
                if (action.bindings[i].effectivePath == newBinding.overridePath)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
