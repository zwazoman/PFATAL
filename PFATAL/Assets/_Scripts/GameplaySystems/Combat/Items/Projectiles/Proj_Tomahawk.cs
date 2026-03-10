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
        _visuals.Rotate(_spinSpeed * Time.deltaTime,0,0);

        base.Update();
    }

    protected override void HitDamageable(DamageData damageData, DamageableObject damageable)
    {
        if(damageable.TryGetComponent(out PlayerPhysics _physics))
        {
            _physics.AddImpulse(damageData.Direction * _knockbackStrength);
        }

        base.HitDamageable(damageData, damageable);
    }
}
