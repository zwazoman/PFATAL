using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using TMPro;
using System.Collections.Generic;

public class Rebinds : MonoBehaviour
{
    public InputActionAsset InputActions;
    public InputActionReference InputReference;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    private InputAction _moveAction;
    [SerializeField] private TMP_Text _rebindLabel;

    private int _bindingIndex;

    
    private void Awake()
    {
        //_moveAction = InputActions.FindAction("Move");
        _moveAction = InputReference;
    }

    // If you want to rebind a specific movement -> put in the string one of those WITHOUT ANY UPPERCASES (PAS DE MAJUSCULES) : up / down / left / right
    public void RebindMovements(string movementToChange)
    {
        InputActions.FindActionMap("Player").Disable();
        _rebindLabel.text = "...";
        //_rebindingOperation = MoveAction.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted()).Start();
        _bindingIndex = InputReference.action.bindings.IndexOf(x => x.isPartOfComposite && x.name == movementToChange);
        _rebindingOperation = InputReference.action.PerformInteractiveRebinding().WithControlsExcluding("Mouse").WithTargetBinding(_bindingIndex).OnMatchWaitForAnother(.2f).OnComplete(operation => RebindCompleted(_moveAction)).Start();
    }

    public void RebindSingleInput(InputAction action)
    {
        InputActions.FindActionMap("Player").Disable();
        //To change later for the script in wich it will do every changes for the UI.
        _rebindLabel.text = "...";
        _rebindingOperation = action.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted(action)).Start();
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
