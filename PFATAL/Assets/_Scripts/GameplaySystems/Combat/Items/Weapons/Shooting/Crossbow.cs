using System;
using _scripts.PlayerCharacter;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crossbow : ProjectileWeapon
{
    [Header("Crossbow Parameters")]

    [SerializeField] float _maxChargeTime = 1.5f;
    [SerializeField] float _chargeZoomThreshold = .3f;
    [SerializeField] Vector2 _CameraRecoilStrength;

    bool isCharged = false;
    float chargeValue;

    //zoom
    private float _cameraFovOffset = 0;
    private float _fovOffsetVelocity = 0;

    protected virtual void Update()
    {

        //update camera zoom
        const float MAX_FOV_ZOOM = 15;
        float alpha = Mathf.Max( (chargeValue - _chargeZoomThreshold) / (1f - _chargeZoomThreshold),0);
        _cameraFovOffset = 
            Mathf.SmoothDamp(_cameraFovOffset, - alpha * MAX_FOV_ZOOM,
                ref _fovOffsetVelocity, .13f,Mathf.Infinity,Time.deltaTime);
        
        _playerCharacter.cameraBehaviour.AddTemporaryFovOffset(_cameraFovOffset);
        
    }

    public override void UseUpdate()
    {
        if (isCharged) return;
        
        //charge shot when holding the click
        chargeValue += Time.deltaTime / _maxChargeTime;
        if(chargeValue >= 1 )
        {
            isCharged = true;
            chargeValue = 1f;
            print("crossbow fully charged");
        }
    }
    
    public override void StopUsing()
    {
        if (!canShoot || !isUsing)
            return;

        //spawn projectile
        SpawnContext spawnContext = new(NetworkManager.Singleton.LocalClientId);
        spawnContext.floatData = chargeValue;
        Shoot(spawnContext);
        
        //recoil
        _playerCharacter.cameraBehaviour.AddRecoil(
            new Vector2(Random.Range(- _CameraRecoilStrength.x, _CameraRecoilStrength.x), _CameraRecoilStrength.y) * (1f+chargeValue));
        
        //reset charge
        chargeValue = 0;
        isCharged = false;

        base.StopUsing();
    }

    void StopCameraZoom()
    {

    }
}
