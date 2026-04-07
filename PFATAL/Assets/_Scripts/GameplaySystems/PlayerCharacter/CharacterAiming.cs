using _scripts.PlayerCharacter;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// la gestion de la rotation de la camera
/// </summary>
public class CharacterAiming : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] PlayerCharacter _character;
    [SerializeField] Transform _cameraRoot;
    [SerializeField] Rigidbody _rigidbody;
    
    [Header("parameters")]
    public float Sensitivity;
    [SerializeField] Vector2 _recoilCompensationMultiplier;

    private float angle = 0;

    private Vector2 _aimAssist;
    private float _inputMultiplier = 1f;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if(GameManager.Instance) GameManager.Instance.EventOnGameEnded += (_) => enabled = false;
    }

    private void LateUpdate()
    {
        //camera rotation
        //_cameraRoot.Rotate(_sensitivity * Time.deltaTime* _character.inputs.aimInput.y * Vector3.right,Space.Self);
        float angleY = (angle + Sensitivity * Time.deltaTime * _character.inputs.aimInput.y * _inputMultiplier);
        if (_character.inputs.UsingGamePad == true)
        {
            angleY -= _aimAssist.y;
            _aimAssist.y = 0f;
            _inputMultiplier = 1f;
        }
        angle = Mathf.Clamp(angleY, -90, 90);
        
        _cameraRoot.transform.localEulerAngles = angle * Vector3.right;
        
        //recoil compensation
        //_character.cameraBehaviour.CompensateRecoil(
        //    Vector2.Scale(_character.inputs.aimInput * (_sensitivity * Time.deltaTime),_recoilCompensationMultiplier));

    }

    void FixedUpdate()
    {
        if (_character.inputs.UsingGamePad == true)
        {
            float angle2 = Sensitivity * Time.deltaTime * _character.inputs.aimInput.x * Mathf.Deg2Rad * _inputMultiplier;
            angle2 += _aimAssist.x;
            _aimAssist.x = 0f;
            _rigidbody.MoveRotation(_rigidbody.rotation * quaternion.RotateY(angle2));
        }
        else
        {
            _rigidbody.MoveRotation(_rigidbody.rotation * quaternion.RotateY(Sensitivity * Time.deltaTime * _character.inputs.aimInput.x * Mathf.Deg2Rad));
        }
        Debug.Log(_inputMultiplier);
    }

    public void AssistAim(Vector2 aimAssist, float inputMultiplier)
    {
        _aimAssist += aimAssist;
        _inputMultiplier = Mathf.Lerp(_inputMultiplier, _inputMultiplier * inputMultiplier, 1f - inputMultiplier);
    }
}
