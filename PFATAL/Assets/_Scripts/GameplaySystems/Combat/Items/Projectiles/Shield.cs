using UnityEngine;

public class Shield : Projectile
{
    [Header("References")]

    [SerializeField] DamageableObject _damageable;

    float _lifetime;

    float _timer = 0;
    bool _despawning = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _damageable.SetMaxHP(spawnContext.Value.floatData2);
        _lifetime = spawnContext.Value.floatData;

        _damageable.OnDie += DestroyShield;
    }

    private void Update()
    {
        if(_timer >= _lifetime && !_despawning)
        {
            _despawning = true;
            Despawn();
        }
    }

    void DestroyShield()
    {
        Despawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        _damageable.OnDie -= DestroyShield;


        _timer = 0;
        _despawning = false;
    }
}
