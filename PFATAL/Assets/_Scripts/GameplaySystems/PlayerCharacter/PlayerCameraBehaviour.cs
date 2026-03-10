using _scripts.PlayerCharacter;
using UnityEngine;

public class PlayerCameraBehaviour : MonoBehaviour
{
    [Header("sceneReferences")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _recoilTarget;
    private Vector3 _vel;

    //FOV scaling
    [Header("FOV scaling")]
    [SerializeField] private Vector2 FOVRange;
    [SerializeField] private float _fovSmoothTime;
    [SerializeField] private float _power = 3;
    private float _fovVel;

    [Header("recoil")]
    [SerializeField] private float _recoilMultiplier;
    [SerializeField] private float _recoilStabilizationSpeed;
    private Vector2 _recoilVector;//en degres
    private Vector2 _recoilVelocity;//en degres


    public void AddRecoil(Vector2 recoil)
    {
        _recoilVelocity += recoil; //?
        _recoilVector += recoil;
    }

    public void CompensateRecoil(Vector2 compensation)
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //fov
        float targetFOV = Mathf.Lerp(FOVRange.x, FOVRange.y,
            Mathf.Pow(_playerCharacter.physics.Velocity.magnitude / _playerCharacter.stateMachine.s_Walking._walkSpeed, _power));
        _cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView,targetFOV,ref _fovVel,_fovSmoothTime);
        
        //recoil stabilisation
        _recoilVector = Vector2.SmoothDamp(_recoilVector,Vector2.zero,ref _recoilVector,_recoilStabilizationSpeed);
    }
}
