using _scripts.PlayerCharacter;
using UnityEngine;

public class PlayerCameraBehaviour : MonoBehaviour
{
    public static float BaseFov = 70;
    [Header("sceneReferences")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _recoilTarget;
    [SerializeField] private Transform _aimPuchBodyCenterReference;

    //FOV scaling
    [Header("FOV scaling")]
    [SerializeField] private float FOVVelocityOffset = 3;
    [SerializeField] private float _fovSmoothTime;
    [SerializeField] private float _playerVelocityToFovScalingCurveExponent = 3;
    
    private float _fovVel;

    [Header("recoil")]
    [SerializeField] private float _recoilMultiplier;
    [SerializeField] private float _recoilStabilizationDuration;
    [SerializeField] [Range(0,1)] private float _recoilSlerpExponent = .95f;

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
        Vector3 worldVector = (_aimPuchBodyCenterReference.position - damageData.Point).normalized;
        Vector2 cameraVector = _cam.worldToCameraMatrix* worldVector
            * damageData.Amount/_playerCharacter.health.MaxHP;
        AddRecoil(Vector2.Scale(cameraVector+_aimPunchDirectionOffset,_aimPunchMultiplier));
    }
    
    public void AddRecoil(Vector2 recoil)
    {
        //Vector2 quarterRecoil = recoil * (_recoilMultiplier * .25f);
        //_recoilVelocity += recoil; //?
        //for(int i = 0; i < 4; i++)
            _recoilVector += recoil * (_recoilMultiplier * 1);
    }

    public void CompensateRecoil(Vector2 compensation)
    {
        //_recoilVelocity += compensation; //?
        float recoilMagnitude = _recoilVector.magnitude;

        //todo : veille destiny + demander à Anfray
        
        //if (recoilMagnitude == 0 || compensation.magnitude == 0) return;
        
        //print(recoilMagnitude);
        //print(compensation);
        //print(-Vector2.Dot(_recoilVector/recoilMagnitude, compensation));
        //print(Mathf.Clamp(-Vector2.Dot(_recoilVector/recoilMagnitude, compensation),0,recoilMagnitude));
        //print(compensation.normalized * Mathf.Clamp(-Vector2.Dot(_recoilVector/recoilMagnitude, compensation),0,recoilMagnitude));
        //_recoilVector += compensation
        //                 * Mathf.Clamp(-Vector2.Dot(_recoilVector/recoilMagnitude, compensation),0,recoilMagnitude);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //fov
        float targetFOV = Mathf.Lerp( BaseFov, BaseFov + FOVVelocityOffset,
            Mathf.Pow(_playerCharacter.physics.Velocity.magnitude / _playerCharacter.stateMachine.s_Walking._walkSpeed, _playerVelocityToFovScalingCurveExponent));
        _cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView,targetFOV,ref _fovVel,_fovSmoothTime);
        
        //recoil stabilisation
        _recoilVector = Vector2.SmoothDamp(_recoilVector,Vector2.zero,ref _recoilVelocity,_recoilStabilizationDuration);
        
        //apply recoil
        _recoilTarget.localRotation = Quaternion.Slerp(
            _recoilTarget.localRotation,
            Quaternion.Euler(_recoilVector.y,_recoilVector.x,0),
            Mathf.Pow(_recoilSlerpExponent,Time.deltaTime)) ;
    }
}
