using Unity.Netcode;
using UnityEngine;

public class CrossbowProjectile : Projectile
{
    [SerializeField] float _speed = 50;
    [SerializeField] float _lifetime = 3;

    bool _move;
    float _timer;

    private void Update()
    {
        if (!IsSpawned || !IsServer)
            return;

        //mouvement

        transform.Translate(transform.forward * _speed * Time.deltaTime,Space.World);

        //lifetime

        _timer += Time.deltaTime;

        if (_timer >= _lifetime)
        {
            DespawnRpc();
            _timer = 0;
        }

        //collisions

        foreach(Collider coll in Physics.OverlapSphere(transform.position, 1))
        {
            if(coll.gameObject.TryGetComponent(out DamageableObject damageable))
            {
                if(damageable.OwnerClientId == spawnContext.askerID)
                {
                    continue;
                }

                DamageData data = new();

                damageable.TakeDamage(data);
                DespawnRpc();
                break;
            }   
        }
    }

    [Rpc(SendTo.Server)]
    void DespawnRpc()
    {
        NetworkObject.Despawn();
    }
}
