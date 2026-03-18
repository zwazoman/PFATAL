using Unity.Netcode;
using UnityEngine;

public class Proj_Crossbow : Proj_Falling
{
    float _initialSpeed;
    float _initialDamage;

    [SerializeField] float _chargeDamageMultiplyer = 2;
    [SerializeField] float _chargeSpeedMultiplyer = 1;

    public override void OnSpawn()
    {
        base.OnSpawn();

        _initialSpeed = speed;
        _initialDamage = damageAmount;

        speed *= 1 + spawnContext.Value.floatData * _chargeSpeedMultiplyer;
        damageAmount *= 1 + spawnContext.Value.floatData * _chargeDamageMultiplyer;
    }

    protected override void Despawn()
    {
        speed = _initialSpeed;
        damageAmount = _initialDamage;

        base.Despawn();
    }

}
