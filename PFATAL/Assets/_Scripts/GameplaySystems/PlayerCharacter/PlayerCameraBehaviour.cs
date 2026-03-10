using _scripts.PlayerCharacter;
using UnityEngine;

public class PlayerCameraBehaviour : MonoBehaviour
{
    [Header("sceneReferences")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _recoilTarget;
    [SerializeField] private Transform _aimPuchBodyCenterReference;

    //FOV scaling
    [Header("FOV scaling")]
    [SerializeField] private Vector2 FOVRange;
    [SerializeField] private float _fovSmoothTime;
    [SerializeField] private float _playerVelocityToFovScalingCurveExponent = 3;
    
    private float _fovVel;

    [Header("recoil")]
    [SerializeField] private float _recoilMultiplier;
    [SerializeField] private float _recoilStabilizationDuration;

    [Header("recoil : aimPunch")]
    [SerializeField] private Vector2 _aimPunchDirectionOffset;
    [SerializeField] private Vector2 _aimPunchMultiplier;
    
    private Vector2 _recoilVector;//en degres
    private Vector2 _recoilVelocity;

    void Awake()
    {
        _playerCharacter.health.OnDamageTaken += ApplyAimPuchRecoil;   
    }

    private void ApplyAimPuchRecoil(DamageData damageData)
    {
        print("ahhhh j'ai maaal au secouuurs je meurs ..");
        Vector3 worldVector = (_aimPuchBodyCenterReference.position - damageData.Point) / damageData.Radius;
        Vector2 cameraVector = _cam.worldToCameraMatrix* worldVector
            * damageData.Amount/_playerCharacter.health.MaxHP;
        AddRecoil(Vector2.Scale(cameraVector+_aimPunchDirectionOffset,_aimPunchMultiplier));
    }
    
    public void AddRecoil(Vector2 recoil)
    {
        //_recoilVelocity += recoil; //?
        _recoilVector += recoil * _recoilMultiplier;
    }

    public void CompensateRecoil(Vector2 compensation)
    {
        //_recoilVelocity += compensation; //?
        _recoilVector += compensation
                         * Mathf.Max(0, -Vector2.Dot(_recoilVector.normalized, compensation));
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //fov
        float targetFOV = Mathf.Lerp(FOVRange.x, FOVRange.y,
            Mathf.Pow(_playerCharacter.physics.Velocity.magnitude / _playerCharacter.stateMachine.s_Walking._walkSpeed, _playerVelocityToFovScalingCurveExponent));
        _cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView,targetFOV,ref _fovVel,_fovSmoothTime);
        
        //recoil stabilisation
        _recoilVector = Vector2.SmoothDamp(_recoilVector,Vector2.zero,ref _recoilVelocity,_recoilStabilizationDuration);
        
        //apply recoil
        _recoilTarget.localRotation = Quaternion.Euler(_recoilVector.y,_recoilVector.x,0);
    }
}
