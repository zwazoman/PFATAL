using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using TMPro;
using System.Collections.Generic;
using _scripts.PlayerCharacter;

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
        if (_menu.activeInHierarchy == true)
        {
            _menu.SetActive(false);
            _playerCharacter.SwapActionMapToPlayer();
        }
        else
        {
            _menu.SetActive(true);
            _playerCharacter.SwapActionMapToUI();
        }
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
        _rebindingOperation = InputReference.action.PerformInteractiveRebinding().WithControlsExcluding("Mouse").WithTargetBinding(_bindingIndex).OnMatchWaitForAnother(.2f).OnComplete(operation => RebindCompleted(_moveAction)).Start();
    }

    public void RebindSingleInput(InputActionReference action)
    {
        _action = action;
        InputActions.FindActionMap("Player").Disable();
        _rebindingOperation = _action.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted(_action)).Start();
    }

    private void RebindCompleted(InputAction action)
    {
        _rebindingOperation.Dispose();

        string newBinding = action.GetBindingDisplayString(_bindingIndex).ToUpper();
        //To change later for the script in wich it will do every changes for the UI.
        _rebindLabel.text = $"{newBinding}";
        InputActions.FindActionMap("Player").Enable();
    }
}
