using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class CrossbowProjectile : Projectile
{
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

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();
    //    //_spawnTime = TimeStamp.Now;
    //    //_spawnPosition = transform.position;
    //    if (NetworkManager.Singleton.IsServer)
    //    {
    //        InitRPC(TimeStamp.Now,transform.position);
    //    }
    //}

    public override void OnSpawn()
    {
        if (NetworkManager.Singleton.IsServer)
            InitRPC(TimeStamp.Now, transform.position, spawnContext.data);
    }

    [Rpc(SendTo.Everyone)]
    void InitRPC(float timestamp, Vector3 position, float chargeTime)
    {
        _spawnTime = timestamp;
        _spawnPosition = position;
        transform.localScale *= (1 + chargeTime); //todo virer et faire l'�quilibrage des dgts et de la vitesse
        _initialized = true;
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
            
            Debug.DrawLine(transform.position, _oldPosition, Color.white,_maxLifetime);
            
            //collisions
            Vector3 movement = transform.position - _oldPosition;

            int hitCount = Physics.SphereCastNonAlloc(
                transform.position, CollisionRadius,
                movement.normalized,
                _hitBuffer,
                movement.magnitude,
                _layerMask);
            
            int actualHitCount = hitCount;
            //print("HitCount : "+hitCount);
            for(int i =0; i < hitCount; i++)
            {
                    print("Collision !!!!!!!! - "+_hitBuffer[i].collider.gameObject);
                
                //print(_hitBuffer[i].collider.gameObject.name);
                if(_hitBuffer[i].collider.gameObject.TryGetComponent(out DamageableObject damageable))
                {
                    print("DamageableObject");
                    
                    if (damageable.OwnerClientId == spawnContext.askerID)
                    {
                        actualHitCount--;
                        continue;
                    }

                    DamageData data = new();
                    data.Point = _hitBuffer[i].point;
                    data.Amount = _damageAmount;
                    data.Radius = 0;
                    data.SourcePlayerClientID = spawnContext.askerID;
                    damageable.TakeDamage(data);
                 
                    DespawnRpc();
                    return;
                }   
            }
            //print("ActualHitCount : "+actualHitCount);
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
