using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class BaseProjectile : Projectile
{
    /// <summary>
    /// l'id du joeur ayant tiré le projectile
    /// </summary>
    [HideInInspector] public ulong spawnerID;

    [Header("settings")]
    [field : SerializeField] public float CollisionRadius { get; private set; }
    [SerializeField] float _speed = 50;
    [SerializeField] float _gravity = 0;
    [SerializeField] float _maxLifetime = 3;
    [SerializeField] private float _damageAmount;
    [SerializeField] LayerMask _layerMask;
    
    private float _spawnTime;
    private Vector3 _spawnPosition;
    
    Vector3 _oldPosition;

    static RaycastHit[] _hitBuffer = new RaycastHit[10];

    private bool _initialized = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        print("ahhhhhhhhh");
        //_spawnTime = TimeStamp.Now;
        //_spawnPosition = transform.position;
        if (NetworkManager.Singleton.IsServer)
        {
            print("bbbbbbb");
            InitRPC(TimeStamp.Now,transform.position);
        }
    }

    [Rpc(SendTo.Everyone)]
    void InitRPC(float timestamp, Vector3 position)
    {
        print("initRPC");
        _spawnTime = timestamp;
        _spawnPosition = position;
        _initialized = true;
        Debug.DrawRay(transform.position, transform.up, Color.red,10000);
    }

    protected virtual void UpdatePosition(float timeSinceSpawn)
    {
        transform.position = _spawnPosition 
                             + transform.forward * (_speed * timeSinceSpawn)
                             + Vector3.up * (timeSinceSpawn * timeSinceSpawn * -.5f * _gravity);
    }
    
    private void Update()
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
            
            Debug.DrawLine(transform.position, _oldPosition, Color.white,1000);
            
            //collisions
            Vector3 movement = transform.position - _oldPosition;
            int hitCount = Physics.SphereCastNonAlloc(transform.position, CollisionRadius, movement, _hitBuffer, movement.magnitude,_layerMask);
            int actualHitCount = hitCount;
            print("HitCount : "+hitCount);
            for(int i =0; i < hitCount; i++)
            {
                print(_hitBuffer[i].collider.gameObject.name);
                if(_hitBuffer[i].collider.gameObject.TryGetComponent(out DamageableObject damageable))
                {
                    if (damageable.OwnerClientId == spawnerID)
                    {
                        actualHitCount--;
                        continue;
                    }

                    DamageData data = new();
                    data.Point = _hitBuffer[i].point;
                    data.Amount = _damageAmount;
                    data.Radius = 0;
                    data.SourcePlayerClientID = spawnerID;
                    damageable.TakeDamage(data);
                 
                    DespawnRpc();
                    return;
                }   
            }
            print("ActualHitCount : "+actualHitCount);
            if(actualHitCount > 0) DespawnRpc();
        }
        
        
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
