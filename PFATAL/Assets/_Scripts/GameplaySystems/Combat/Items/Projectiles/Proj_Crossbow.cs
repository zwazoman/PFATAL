using _Scripts.Pooling;
using UnityEngine;

public class Proj_Crossbow : Proj_Falling
{
    float _initialSpeed;
    float _initialDamage;

    [SerializeField] float _chargeDamageMultiplyer = 2;
    [SerializeField] float _chargeSpeedMultiplyer = 1;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _initialSpeed = speed;
        _initialDamage = damageAmount;

        speed *= 1 + spawnContext.Value.floatData * _chargeSpeedMultiplyer;
        damageAmount *= 1 + spawnContext.Value.floatData * _chargeDamageMultiplyer;
    }

    public override void Despawn()
    {
        speed = _initialSpeed;
        damageAmount = _initialDamage;
        
        base.Despawn();
    }

    protected override void OnContact(RaycastHit hit)
    {
        print("OnContact !!!");
        PooledObject vfx = LocalPoolManager.Instance.Pool_VFX_Hit_Crossbow.PullObjectFromPool(hit.point);
        vfx.transform.up = hit.normal;
        vfx.GoBackIntoPool_Delayed(2);
        base.OnContact(hit);
    }
}
