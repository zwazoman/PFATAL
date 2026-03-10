using _scripts.PlayerCharacter;
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
    [SerializeField] float _sensitivity;
    [SerializeField] Vector2 _recoilCompensationMultiplier;

    private float angle = 0;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if(GameManager.Instance) GameManager.Instance.EventOnGameEnded += (_) => enabled = false;
    }

    private void LateUpdate()
    {
        //camera rotation
        //_cameraRoot.Rotate(_sensitivity * Time.deltaTime* _character.inputs.aimInput.y * Vector3.right,Space.Self);
        angle = Mathf.Clamp((angle + _sensitivity * Time.deltaTime * _character.inputs.aimInput.y), -90,90);
        _cameraRoot.transform.localEulerAngles = angle * Vector3.right;
        
        //recoil compensation
        _character.cameraBehaviour.CompensateRecoil(
            Vector2.Scale(_character.inputs.aimInput * (_sensitivity * Time.deltaTime),_recoilCompensationMultiplier));

    }

    void FixedUpdate()
    {
        _rigidbody.MoveRotation(_rigidbody.rotation * quaternion.RotateY( _sensitivity * Time.deltaTime * _character.inputs.aimInput.x * Mathf.Deg2Rad));
    }
}
