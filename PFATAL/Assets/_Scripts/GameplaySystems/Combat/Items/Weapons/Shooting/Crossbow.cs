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
    [SerializeField] Vector2 _CameraRecoilStrength;

    bool _startedCharging = false;
    bool _isCharged = false;
    float _chargeValue;

    //zoom
    private float _cameraFovOffset = 0;
    private float _fovOffsetVelocity = 0;

    public override void StartUsing()
    {
        if (canShoot)
        {
            base.StartUsing();
        }
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
        if (_isCharged) return;
        
        //charge shot when holding the click
        _chargeValue += Time.deltaTime / _maxChargeTime;

        if(_chargeValue >= _chargeStartThreshold && ! _startedCharging)
        {
            OnStartCharging?.Invoke();
            _startedCharging = true;
        }
        
        if(_chargeValue >= 1 )
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
        Shoot(spawnContext);

        //recoil
        playerCharacter.cameraBehaviour.AddRecoil(
            new Vector2(Random.Range(- _CameraRecoilStrength.x, _CameraRecoilStrength.x), _CameraRecoilStrength.y) * (1f+_chargeValue));
        
        //reset charge
        _chargeValue = 0;
        _isCharged = false;
        _startedCharging = false;

        base.StopUsing();
    }

    protected override Awaitable<GameObject> Shoot(SpawnContext spawnContext)
    {
        OnCrossbowShoot?.Invoke(_chargeValue);

        return base.Shoot(spawnContext);
    }
}
