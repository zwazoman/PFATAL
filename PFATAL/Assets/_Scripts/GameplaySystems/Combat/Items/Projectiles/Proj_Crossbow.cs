using Unity.Netcode;
using UnityEngine;

public class Proj_Crossbow : Proj_Falling
{
    float _initialSpeed;
    float _initialDamage;

    [SerializeField] float _chargeDamageMultiplyer = 1;
    [SerializeField] float _chargeSpeedMultiplyer = 1;

    public override void OnSpawn()
    {
        base.OnSpawn();

        _initialSpeed = speed;
        _initialDamage = damageAmount;
    }

    protected override void Despawn()
    {
        speed = _initialSpeed;
        damageAmount = _initialDamage;

        base.Despawn();
    }

}
