using Unity.Netcode;
using UnityEngine;

public class ToxicCloud : NetworkBehaviour
{
    private static Collider[] buffer = new Collider[20];

    [SerializeField] float _radius = 4f;
    [SerializeField] float _duration = 6f;
    [SerializeField] float _tickRate = 0.5f;
    [SerializeField] float _damagePerTick = 1f;

    float _timer;
    float _tickTimer;

    ulong _ownerId;

    public void Init(ulong ownerId)
    {
        _ownerId = ownerId;
    }

    private void Update()
    {
        if (!IsServer) return;

        _timer += Time.deltaTime;
        _tickTimer += Time.deltaTime;

        if (_tickTimer >= _tickRate)
        {
            _tickTimer = 0f;
            ApplyDamage();
        }

        if (_timer >= _duration)
        {
            NetworkObject.Despawn();
        }
    }

    void ApplyDamage()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, _radius, buffer);

        for (int i = 0; i < count; i++)
        {
            if (buffer[i].TryGetComponent(out DamageableObject hit))
            {
                DamageData damage = new DamageData
                {
                    Amount = _damagePerTick,
                    SourcePlayerClientID = _ownerId,
                    Point = hit.transform.position,
                    Direction = Vector3.zero,
                    KnockbackForce = Vector3.zero,
                    Radius = _radius
                };

                hit.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
