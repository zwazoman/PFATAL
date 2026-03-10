using _scripts.PlayerCharacter;
using Unity.Netcode;
using UnityEngine;

public class Crossbow : ProjectileWeapon
{
    [Header("Crossbow Parameters")]

    [SerializeField] float _maxChargeTime = 1.5f;
    [SerializeField] float _chargeZoomThreshold = .3f;

    bool isCharged = false;
    float chargeValue;

    public override void UseUpdate()
    {
        if (isCharged)
            return;

        chargeValue += Time.deltaTime / _maxChargeTime;

        if(chargeValue >= 1)
        {
            isCharged = true;
            chargeValue = _maxChargeTime;
            print("crossbow fully charged");
        }
        else if(chargeValue >= _chargeZoomThreshold)
        {
            //début zoom caméra
        }
    }

    public override void StopUsing()
    {
        if (!canShoot || !isUsing)
            return;

        SpawnContext spawnContext = new(NetworkManager.Singleton.LocalClientId);
        spawnContext.floatData = chargeValue;

        Shoot(spawnContext);
        chargeValue = 0;
        isCharged = false;

        base.StopUsing();
    }

    void StopCameraZoom()
    {

    }
}
