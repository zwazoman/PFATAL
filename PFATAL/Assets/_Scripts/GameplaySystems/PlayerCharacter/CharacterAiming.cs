using _scripts.PlayerCharacter;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// la gestion de la rotation de la camera
/// </summary>
public class CharacterAiming : NetworkBehaviour
{
    [Header("Scene References")]
    [SerializeField] PlayerCharacter _character;
    [SerializeField] Transform _cameraRoot;
    [SerializeField] Rigidbody _rigidbody;
    
    [Header("parameters")]
    public float Sensitivity;
    public float ControllerSensitivity;
    [SerializeField] Vector2 _recoilCompensationMultiplier;

    private float angle = 0;

    [Header("Controller")]
    private Vector2 _aimAssist;
    private float _inputMultiplier = 1f;
    [SerializeField][Range(0.5f,3f)] private float _aimAssistYStrength = 1f;
    private float _previousControllerSensitivity;
    [SerializeField] private float _controllerSensitivityMaxAcceleration;
    [SerializeField][Range(0.001f,0.5f)] private float _startAcceleration;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if(GameManager.Instance) GameManager.Instance.EventOnGameEnded += (_) => enabled = false;
        _previousControllerSensitivity = ControllerSensitivity;
        InputSystem.pollingFrequency = 120;
    }


    void ApplyInputsToCameraAndCharacterRotation()
    {
        //camera rotation
        //_cameraRoot.Rotate(_sensitivity * Time.deltaTime* _character.inputs.aimInput.y * Vector3.right,Space.Self);
        if (_character.inputs.UsingGamePad == true)
        {
            angle = (angle + ControllerSensitivity * Time.deltaTime * _character.inputs.aimInput.y * _inputMultiplier);
            float angle1 = angle;
            if (angle1 < 0) 
                angle1 *= -1;

            angle -= (angle1 * _aimAssist.y) * _aimAssistYStrength;
            _aimAssist.y = 0f;
            _inputMultiplier = 1f;

            StartCoroutine(Accelerate());
        }
        else
        {
            angle = (angle + Sensitivity * Time.deltaTime * _character.inputs.aimInput.y * _inputMultiplier);
        }
        angle = Mathf.Clamp(angle, -90, 90);
        
        _cameraRoot.transform.localEulerAngles = angle * Vector3.right;
        
        //recoil compensation
        //_character.cameraBehaviour.CompensateRecoil(
        //    Vector2.Scale(_character.inputs.aimInput * (_sensitivity * Time.deltaTime),_recoilCompensationMultiplier));
    }
    
    private void LateUpdate()
    {
        //offline controller
        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            ApplyInputsToCameraAndCharacterRotation();
            return;
        }
        
        //owner
        if (IsOwner )
        {
            ApplyInputsToCameraAndCharacterRotation();
            ReplicateAimPitchRPC(angle);
        }
    }

    [Rpc(SendTo.NotOwner)]
    private void ReplicateAimPitchRPC(float xAngle)
    {
        _cameraRoot.transform.localEulerAngles = xAngle * Vector3.right;
    }

    void FixedUpdate()
    {
        if (_character.inputs.UsingGamePad == true)
        {
            float angle2 = ControllerSensitivity * Time.deltaTime * _character.inputs.aimInput.x * Mathf.Deg2Rad * _inputMultiplier;
            angle2 += _aimAssist.x;
            _aimAssist.x = 0f;
            _rigidbody.MoveRotation(_rigidbody.rotation * quaternion.RotateY(angle2));
        }
        else
        {
            _rigidbody.MoveRotation(_rigidbody.rotation * quaternion.RotateY(Sensitivity * Time.deltaTime * _character.inputs.aimInput.x * Mathf.Deg2Rad));
        }
    }

    public void AssistAim(Vector2 aimAssist, float inputMultiplier)
    {
        _aimAssist += aimAssist;
        _inputMultiplier = Mathf.Lerp(_inputMultiplier, _inputMultiplier * inputMultiplier, 1f - inputMultiplier);
    }

    IEnumerator Accelerate()
    {
        Gamepad gamepad = Gamepad.current;
        yield return new WaitForSeconds(_startAcceleration);
        if (_character.inputs.UsingGamePad == true)
        {
            if (((gamepad.rightStick.ReadValue().x > 0.7f) || (gamepad.rightStick.ReadValue().y > 0.7f) ||
                (gamepad.rightStick.ReadValue().x < -0.7f) || (gamepad.rightStick.ReadValue().y < -0.7f)) && GetComponent<PlayerDetector>()._canDecrease == true)
            {
                while (ControllerSensitivity < _controllerSensitivityMaxAcceleration)
                {
                    ControllerSensitivity += 0.01f;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else if (GetComponent<PlayerDetector>()._canDecrease)
            {
                ControllerSensitivity = _previousControllerSensitivity;
                StopAllCoroutines();
            }
            else
            {
                StopAllCoroutines();
            }
        }
        else
        {
            ControllerSensitivity = _previousControllerSensitivity;
            StopAllCoroutines();
        }
    }
}
