using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Proj_Falling : Projectile
{
    [Header("Global Settings")]
    [field : SerializeField] public float CollisionRadius { get; private set; }
    [SerializeField] float _speed = 50;
    [SerializeField] float _gravity = 0;
    [SerializeField] float _maxLifetime = 3;
    [SerializeField] float _damageAmount;
    [SerializeField] float _chargeDamageMultiplyer = 1;
    [SerializeField] float _chargeSpeedMultiplyer = 1;
    [SerializeField] LayerMask _layerMask;

    float _spawnTime;
    Vector3 _spawnPosition;
    
    Vector3 _oldPosition;

    static RaycastHit[] _hitBuffer = new RaycastHit[10];

    bool _initialized = false;

    public override void OnSpawn()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            InitRPC(TimeStamp.Now, transform.position, spawnContext.floatData);
            _damageAmount *= (1 + spawnContext.floatData) * _chargeDamageMultiplyer;
        }
    }

    [Rpc(SendTo.Everyone)]
    void InitRPC(float timestamp, Vector3 position, float chargeTime)
    {
        _spawnTime = timestamp;
        _spawnPosition = position;
        _speed *= (1 + chargeTime) * _chargeSpeedMultiplyer;
        _initialized = true;
    }

    protected virtual void UpdatePosition(float timeSinceSpawn)
    {
        transform.position = _spawnPosition 
                             + transform.forward * (_speed * timeSinceSpawn)
                             + Vector3.up * (timeSinceSpawn * timeSinceSpawn * -.5f * _gravity);
    }
    
    protected virtual void Update()
    {
        if (!IsSpawned || !_initialized)
            return;
        
        float timeSinceSpawn = TimeStamp.Now-_spawnTime;

        //mouvement
        _oldPosition = transform.position;
        UpdatePosition(timeSinceSpawn);
        
        if (IsServer)
        {
            //lifetime
            if (timeSinceSpawn >= _maxLifetime)
                DespawnRpc();
            
            Debug.DrawLine(transform.position, _oldPosition, Color.white,_maxLifetime);
            
            //collisions
            Vector3 movement = transform.position - _oldPosition;

            int hitCount = Physics.SphereCastNonAlloc(transform.position, CollisionRadius, movement.normalized, _hitBuffer, movement.magnitude,_layerMask);
            int actualHitCount = hitCount;
            //print("HitCount : "+hitCount);
            for(int i =0; i < hitCount; i++)
            {
                print(_hitBuffer[i].collider.gameObject.name);
                if (_hitBuffer[i].collider.gameObject.TryGetComponent(out DamageableObject damageable))
                {
                    if (damageable.OwnerClientId == spawnContext.spawnerClientID && damageable.isPlayer)
                    {
                        actualHitCount--;
                        continue;
                    }
                    DamageData data = new();
                    data.Point = _hitBuffer[i].point;
                    data.Direction = transform.forward;
                    data.Amount = _damageAmount;
                    data.Radius = 1;
                    data.SourcePlayerClientID = spawnContext.spawnerClientID;
                    HitDamageable(data, damageable);

                    DespawnRpc();
                    return;
                }   
            }
            //print("ActualHitCount : "+actualHitCount);
            if(actualHitCount > 0) DespawnRpc();
        }
    }

    protected virtual void HitDamageable(DamageData damageData, DamageableObject damageable)
    {
        damageable.TakeDamage(damageData);
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
