using NetworkTime;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Proj_Falling : Projectile
{
    [Header("Global Settings")]
    [field: SerializeField] public float CollisionRadius { get; private set; }
    [SerializeField] protected float speed = 50;
    [SerializeField] protected float gravity = 0;
    [SerializeField] protected float damageAmount = 1f;
    [SerializeField] float _maxLifetime = 3;
    [SerializeField] LayerMask _layerMask;

    float _spawnTime;
    Vector3 _spawnPosition;

    Vector3 _oldPosition;

    static RaycastHit[] _hitBuffer = new RaycastHit[10];

    protected bool _initialized = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _spawnTime = spawnContext.Value.spawnTime;
        _spawnPosition = spawnContext.Value.spawnPos;

        speed *= 1 + spawnContext.Value.floatData;
        damageAmount *= 1 + spawnContext.Value.floatData * 2;

        _initialized = true;
    }

    public override void OnSpawn()
    {
        _spawnTime = spawnContext.Value.spawnTime;
        _spawnPosition = spawnContext.Value.spawnPos;

        if (NetworkManager.Singleton.IsServer)
        {
            //todo : lag au spawn vient de là ????
            InitRPC(TimeStamp.Now, transform.position, spawnContext.Value.floatData);
        }
    }

    [Rpc(SendTo.Everyone)]
    //todo : sale de faire passer la charge de l'arbalete ici, et la RPC est sale de base en fait, faut trouver un moyen de l'enlever.
    void InitRPC(float timestamp, Vector3 position, float normalizedChargeTime)
    {
        //_spawnTime = timestamp;
        //_spawnPosition = position;

        //sale de zinzin j'ai honte
        if (this is Proj_Crossbow)
        {
            //les fleches chargées font 3x plus de degats et vont 2 fois plus vite
            speed *= 1 + normalizedChargeTime;
            damageAmount *= 1 + normalizedChargeTime * 2;
        }

        _initialized = true;
    }

    protected virtual void UpdatePosition(float timeSinceSpawn)
    {
        transform.position = _spawnPosition
                             + transform.forward * (speed * timeSinceSpawn)
                             + Vector3.up * (timeSinceSpawn * timeSinceSpawn * -.5f * gravity);
    }

    protected virtual void Update()
    {
        if (!IsSpawned || !_initialized)
            return;

        float timeSinceSpawn = TimeStamp.Now - _spawnTime;

        //mouvement
        _oldPosition = transform.position;
        UpdatePosition(timeSinceSpawn);

        if (IsServer)
        {
            //lifetime
            if (timeSinceSpawn >= _maxLifetime)
                Despawn();


            Debug.DrawLine(transform.position, _oldPosition, Color.white, _maxLifetime);

            //collisions
            Vector3 movement = transform.position - _oldPosition;

            int hitCount = Physics.SphereCastNonAlloc(transform.position, CollisionRadius, movement.normalized, _hitBuffer, movement.magnitude, _layerMask);
            int actualHitCount = hitCount;
            //print("HitCount : "+hitCount);
            for (int i = 0; i < hitCount; i++)
            {
                print(_hitBuffer[i].collider.gameObject.name);
                if (_hitBuffer[i].collider.gameObject.TryGetComponent(out DamageableObject damageable))
                {
                    if (damageable.OwnerClientId == spawnContext.Value.spawnerClientID && damageable.isPlayer)
                    {
                        actualHitCount--;
                        continue;
                    }
                    DamageData data = new();
                    data.Point = _hitBuffer[i].point;
                    data.Direction = transform.forward;
                    data.Amount = damageAmount;
                    data.Radius = 1;
                    data.SourcePlayerClientID = spawnContext.Value.spawnerClientID;
                    HitDamageable(data, damageable);

                    Despawn();
                    return;
                }
            }
            //print("ActualHitCount : "+actualHitCount);
            if (actualHitCount > 0) Despawn();
        }
    }

    protected virtual void HitDamageable(DamageData damageData, DamageableObject damageable)
    {
        damageable.TakeDamage(damageData);
    }

    protected virtual void Despawn()
    {
        _initialized = false;
        DespawnRpc();
    }

    [Rpc(SendTo.Server)]
    void DespawnRpc()
    {
        NetworkObject.Despawn();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CollisionRadius);
    }
#endif
}
