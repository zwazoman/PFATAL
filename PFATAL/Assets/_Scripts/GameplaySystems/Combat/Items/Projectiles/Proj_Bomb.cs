using _Scripts.Exceptions;
using UnityEngine;

public class Proj_Bomb : Projectile
{
    [Header("scene references")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] Explosion _explosion;

    [Header("Settings")]
    [SerializeField] float _fuseTime = 3f;

    [SerializeField] private float _throwStrength = 25;
    float _timer;
    private bool _isExploding = false;


    private void Awake()
    {
        TryGetComponent(out  _rb);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

        _isExploding = false;
        _timer = 0f;
        _rb.isKinematic = false;
        Vector3 force = transform.forward * 5 + transform.up * 3;
        _rb.AddForce(force.normalized * _throwStrength, ForceMode.Impulse);
    }

    private void Update()
    {
        if (!IsSpawned) return;
        if (!IsServer) return;
        
        _timer += Time.deltaTime;
        if(_timer >= _fuseTime&& !_isExploding)
        {
            ExplodeAndDespawn();
        }
    }

    async Awaitable ExplodeAndDespawn()
    {
        if (!IsServer) throw new NetworkAuthorityException();
        _isExploding = true;
        _rb.isKinematic = true;

        await _explosion.Explode(spawnContext.Value.spawnerClientID, (int)spawnContext.Value.floatData2);
        
        NetworkObject.Despawn();
    }

    
}
