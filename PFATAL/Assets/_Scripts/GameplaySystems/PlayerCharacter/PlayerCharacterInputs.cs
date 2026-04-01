using Chat;
using System;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// cette classe contient les inputs du joueur et est utilisée dans les states du personnage.
/// </summary>
public class PlayerCharacterInputs : NetworkBehaviour
{
    public event Action OnRespawnInput;

    [HideInInspector] public Vector2 movementInput = Vector2.zero;
    [HideInInspector] public Vector2 aimInput = Vector2.zero;
    [HideInInspector] public bool isHoldingRunKey { get; private set; } = false;

    [Header("Settings")]
    [SerializeField] private float _aimSmoothingTime = .1f;
    [SerializeField] private float _jumpBufferingDuration = .2f;

    private Vector2 aimVel;

    private bool _paused = false;

    private Gamepad _gamepad;
    [HideInInspector] public bool UsingGamePad = false;

    public bool TryConsumeJumpKeyPress()
    {
        bool wasBuffered = _jumpKeyBuffered;
        _jumpKeyBuffered = false;
        return wasBuffered;
    }
    
    private float _lastJumpKeyPressTime;
    private bool _jumpKeyBuffered;
    public bool IsHoldingJumpKey { get; private set; }

    public void Move(InputAction.CallbackContext context)
    {
        _gamepad = Gamepad.current;
        movementInput = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        _gamepad = Gamepad.current;
        if (_gamepad != null)
        {
            if (context.action.activeControl.device.name == _gamepad.name)
            {
                aimInput = context.ReadValue<Vector2>() * 6.5f;
                UsingGamePad = true;
            }
            else
            {
                UsingGamePad = false;
                aimInput = context.ReadValue<Vector2>();
            }
        }
        else
        {
            UsingGamePad = false;
            aimInput = context.ReadValue<Vector2>();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        IsHoldingJumpKey = true;
        if (context.started)
        {
            _jumpKeyBuffered = true;
            _lastJumpKeyPressTime = Time.time;
        }
        _jumpKeyBuffered &= Time.time - _lastJumpKeyPressTime <= _jumpBufferingDuration && IsHoldingJumpKey;
        if (context.canceled)
        {
            IsHoldingJumpKey = false;
            _jumpKeyBuffered = false;
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        isHoldingRunKey = true;
        if (context.canceled)
        {
            isHoldingRunKey = false;
        }
    }

    public void Respawn(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            Respawn();
    }

    public void Respawn() { OnRespawnInput?.Invoke(); }

    void Update()
    {
        if (IsSpawned && !IsOwner) return;
        if (_paused) return;
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameChat.Instance.Show();
            Clear();
            _paused = true;
        }

        //aim, needs fixing with diagonals
        if (UsingGamePad == false)
        {
            aimInput = Vector2.SmoothDamp(
            new Vector2(aimInput.x, -aimInput.y),
            new Vector2(Input.mousePositionDelta.x / (float)Screen.height, Input.mousePositionDelta.y / (float)Screen.height),
            ref aimVel,
            _aimSmoothingTime);
        }
        else
        {
            aimInput = Vector2.SmoothDamp(
            new Vector2(aimInput.x, aimInput.y),
            new Vector2(Input.mousePositionDelta.x / (float)Screen.height, Input.mousePositionDelta.y / (float)Screen.height),
            ref aimVel,
            _aimSmoothingTime);
        }
        
        /*
        aimInput = Vector2.SmoothDamp(
            new Vector2(aimInput.x, -aimInput.y),
            new Vector2(aimInput.x, -aimInput.y),
            ref aimVel,
            _aimSmoothingTime);*/
    }

    public void Clear()
    {
        movementInput = Vector2.zero;
        aimInput = Vector2.zero;
        IsHoldingJumpKey = false;
        _jumpKeyBuffered = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerCharacterInputs))]
public class PlayerCharacterInputsEditor : Editor
{
    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlayerCharacterInputs i = target as PlayerCharacterInputs;
        GUILayout.Label($"Movement Input : {i.movementInput.x}, {i.movementInput.y}");
        GUILayout.Label($"Aim Input : {i.aimInput.x}, {i.aimInput.y}");
    }
}
#endif