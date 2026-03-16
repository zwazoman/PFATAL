using UnityEngine;

public class Proj_Tomahawk : Proj_Falling
{
    [Header("Tomahawk Refs")]
    [SerializeField] Transform _visuals;

    [Header("Tomahawk Settings")]
    [SerializeField] float _spinSpeed = 200;
    [SerializeField] float _knockbackStrength = 20;


    public override void OnSpawn()
    {
        base.OnSpawn();
    }

    protected override void Update()
    {
        if(_initialized)
            _visuals.Rotate(_spinSpeed * Time.deltaTime,0,0);

        base.Update();
    }

    protected override void HitDamageable(DamageData damageData, DamageableObject damageable)
    {
        damageData.KnockbackForce = transform.forward * _knockbackStrength;

        base.HitDamageable(damageData, damageable);
    }
}
