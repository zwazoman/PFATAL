using _Scripts.Exceptions;
using System;
using UnityEngine;

public class Proj_Bomb : Projectile
{
    public event Action OnExplode;

    [Header("scene references")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] Explosion _explosion;

    [Header("Settings")]
    [SerializeField] public float fuseTime = 3f;

    [SerializeField] private float _throwStrength = 25;
    
    [HideInInspector] public float fuseTimer;

    private bool _isExploding = false;


    private void Awake()
    {
        TryGetComponent(out  _rb);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

        fuseTimer = spawnContext.Value.floatData;


        _isExploding = false;
        _rb.isKinematic = false;
        Vector3 force = transform.forward * 5 + transform.up * 3;
        _rb.AddForce(force.normalized * _throwStrength, ForceMode.Impulse);
    }

    private void Update()
    {
        if (!IsSpawned) return;
        if (!IsServer) return;
        
        fuseTimer += Time.deltaTime;
        if(fuseTimer >= fuseTime&& !_isExploding)
        {
            ExplodeAndDespawn();
        }
    }

    async Awaitable ExplodeAndDespawn()
    {
        OnExplode?.Invoke();

        if (!IsServer) throw new NetworkAuthorityException();
        _isExploding = true;
        _rb.isKinematic = true;

        await _explosion.Explode(spawnContext.Value.spawnerClientID, (int)spawnContext.Value.floatData2);

        Despawn();
    }

    
}
