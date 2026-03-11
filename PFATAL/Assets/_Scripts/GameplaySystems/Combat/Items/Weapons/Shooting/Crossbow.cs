using _scripts.PlayerCharacter;
using Unity.Netcode;
using UnityEngine;

public class Crossbow : ProjectileWeapon
{
    [Header("Crossbow Parameters")]

    [SerializeField] float _maxChargeTime = 1.5f;
    [SerializeField] float _chargeZoomThreshold = .3f;
    [SerializeField] Vector2 _CameraRecoilStrength;

    bool isCharged = false;
    float chargeValue;

    public override void UseUpdate()
    {
        if (!isCharged)
        {
            chargeValue += Time.deltaTime / _maxChargeTime;
        }
            


        if(chargeValue >= 1 && !isCharged)
        {
            isCharged = true;
            chargeValue = _maxChargeTime;
            print("crossbow fully charged");
        }
        else if(chargeValue >= _chargeZoomThreshold)
        {
            float alpha = chargeValue / (1f - _chargeZoomThreshold);
            _playerCharacter.cameraBehaviour.AddTemporaryFovOffset(alpha * -15);
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
            new Vector2(Random.Range(- _CameraRecoilStrength.x, _CameraRecoilStrength.x), _CameraRecoilStrength.y)
            );
        
        //reset charge
        chargeValue = 0;
        isCharged = false;

        base.StopUsing();
    }

    void StopCameraZoom()
    {

    }
}
