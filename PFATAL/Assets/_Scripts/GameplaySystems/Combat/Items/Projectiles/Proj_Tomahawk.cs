using UnityEngine;

public class Proj_Tomahawk : Proj_Falling
{
    [Header("Tomahawk Refs")]
    [SerializeField] Transform _visuals;
    [SerializeField] Explosion _explosion;

    [Header("Tomahawk Settings")]
    [SerializeField] float _spinSpeed = 200;
    [SerializeField] float _knockbackStrength = 20;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnContact += Explode;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        OnContact -= Explode;
    }

    protected override void Update()
    {
        if(_initialized)
            _visuals.Rotate(_spinSpeed * Time.deltaTime,0,0);

        base.Update();
    }

    protected override void ApplyDamageToHitObject(DamageData damageData, DamageableObject damageable)
    {
        damageData.KnockbackForce = transform.forward * _knockbackStrength;

        base.ApplyDamageToHitObject(damageData, damageable);
    }

    void Explode()
    {
        print("explode");

        _explosion.Explode(spawnContext.Value.spawnerClientID);
    }
}
