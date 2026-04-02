using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crossbow : ProjectileWeapon
{
    public event Action OnCharged;
    public event Action OnStartCharging;

    public event Action<float> OnCrossbowShoot;

    [Header("Crossbow Parameters")]

    [SerializeField] float _maxChargeTime = 1.5f;
    [SerializeField] float _chargeZoomThreshold = .3f;
    [SerializeField] float _chargeStartThreshold = .1f;
    [SerializeField] float _chargeSpeedMaxMultiplyer = .7f;
    [SerializeField] Vector2 _CameraRecoilStrength;

    bool _startedCharging = false;
    bool _isCharged = false;
    float _chargeValue;

    //zoom
    private float _cameraFovOffset = 0;
    private float _fovOffsetVelocity = 0;

    public override void UnEquip()
    {
        base.UnEquip();

        _chargeValue = 0;
        _isCharged = false;
        _startedCharging = false;

        _cameraFovOffset = 0;
        _fovOffsetVelocity = 0;

        playerCharacter.movement.globalMovespeedMultiplyer = 1;
    }

    protected virtual void Update()
    {

        //update camera zoom
        const float MAX_FOV_ZOOM = 15;
        float alpha = Mathf.Max( (_chargeValue - _chargeZoomThreshold) / (1f - _chargeZoomThreshold),0);
        _cameraFovOffset = 
            Mathf.SmoothDamp(_cameraFovOffset, - alpha * MAX_FOV_ZOOM,
                ref _fovOffsetVelocity, .13f,Mathf.Infinity,Time.deltaTime);
        
        playerCharacter.cameraBehaviour.AddTemporaryFovOffset(_cameraFovOffset);
    }

    public override void UseUpdate()
    {
        if (_isCharged || ! canShoot) return;
        
        //charge shot when holding the click
        _chargeValue += Time.deltaTime / _maxChargeTime;

        if(_chargeValue >= _chargeStartThreshold && ! _startedCharging)
        {
            OnStartCharging?.Invoke();
            _startedCharging = true;
        }

        playerCharacter.movement.globalMovespeedMultiplyer =  1 - _chargeSpeedMaxMultiplyer * _chargeValue;

        if (_chargeValue >= 1 )
        {
            OnCharged?.Invoke();

            _isCharged = true;
            _chargeValue = 1f;
        }
    }
    
    public override void StopUsing()
    {
        if (!canShoot || !isUsing)
            return;

        //spawn projectile
        SpawnContext spawnContext = new(NetworkManager.Singleton.LocalClientId);
        spawnContext.floatData = _chargeValue;
        Shoot(spawnContext, ComputeProjectileRotation());

        //recoil
        playerCharacter.cameraBehaviour.AddRecoil(
            new Vector2(Random.Range(- _CameraRecoilStrength.x, _CameraRecoilStrength.x), _CameraRecoilStrength.y) * (1f+_chargeValue));
        
        //reset charge
        _chargeValue = 0;
        _isCharged = false;
        _startedCharging = false;

        playerCharacter.movement.globalMovespeedMultiplyer = 1;


        base.StopUsing();
    }

    protected override Awaitable<GameObject> Shoot(SpawnContext spawnContext,Quaternion rotation)
    {
        OnCrossbowShoot?.Invoke(_chargeValue);

        return base.Shoot(spawnContext, rotation);
    }
}
