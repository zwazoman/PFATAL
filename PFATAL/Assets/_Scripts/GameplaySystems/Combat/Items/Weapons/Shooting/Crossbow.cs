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

    public float chargeValue;

    //zoom
    private float _cameraFovOffset = 0;
    private float _fovOffsetVelocity = 0;

    public override void Equip()
    {
        base.Equip();

        try
        {
            hand.EquipSpecific(this);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public override void UnEquip()
    {
        base.UnEquip();

        chargeValue = 0;
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
        float alpha = Mathf.Max( (chargeValue - _chargeZoomThreshold) / (1f - _chargeZoomThreshold),0);
        _cameraFovOffset = 
            Mathf.SmoothDamp(_cameraFovOffset, - alpha * MAX_FOV_ZOOM,
                ref _fovOffsetVelocity, .13f,Mathf.Infinity,Time.deltaTime);
        
        playerCharacter.cameraBehaviour.AddTemporaryFovOffset(_cameraFovOffset);
    }

    public override void UseUpdate()
    {
        if (_isCharged || !canShoot) return;
        
        //charge shot when holding the click
        chargeValue += Time.deltaTime / _maxChargeTime;

        if(chargeValue >= _chargeStartThreshold && ! _startedCharging)
        {
            OnStartCharging?.Invoke();
            _startedCharging = true;
        }

        playerCharacter.movement.globalMovespeedMultiplyer =  1 - _chargeSpeedMaxMultiplyer * chargeValue;

        if (chargeValue >= 1 )
        {
            OnCharged?.Invoke();

            _isCharged = true;
            chargeValue = 1f;
        }
    }
    
    public override void StopUsing()
    {
        base.StopUsing();

        if (!canShoot)
            return;

        //spawn projectile
        SpawnContext spawnContext = new(NetworkManager.Singleton.LocalClientId);
        spawnContext.floatData = chargeValue;
        spawnContext.floatData2 = ItemID;
        Shoot(spawnContext, Quaternion.identity, playerCharacter.playerCamera.transform.position);

        //recoil
        playerCharacter.cameraBehaviour.AddRecoil(
            new Vector2(Random.Range(- _CameraRecoilStrength.x, _CameraRecoilStrength.x), _CameraRecoilStrength.y) * (1f+chargeValue));
        
        //reset charge
        chargeValue = 0;
        _isCharged = false;
        _startedCharging = false;

        playerCharacter.movement.globalMovespeedMultiplyer = 1;
    }

    protected override Awaitable<GameObject> Shoot(SpawnContext spawnContext,Quaternion rotation, Vector3 spawnPos)
    {
        OnCrossbowShoot?.Invoke(chargeValue);

        return base.Shoot(spawnContext, rotation, spawnPos);
    }
}
