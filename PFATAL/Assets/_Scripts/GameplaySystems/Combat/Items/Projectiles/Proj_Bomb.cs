using _Scripts.Exceptions;
using Unity.Netcode;
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
    

    private void Awake()
    {
        TryGetComponent(out  _rb);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

        _timer = 0f;
        Vector3 force = transform.forward * 5 + transform.up * 3;
        _rb.AddForce(force * _throwStrength, ForceMode.Impulse);
    }

    private void Update()
    {
        if (!IsSpawned) return;
        if (!IsServer) return;
        
        _timer += Time.deltaTime;
        if(_timer >= _fuseTime)
        {
            Explode();
            NetworkObject.Despawn();
        }
    }

    void Explode()
    {
        if (!IsServer) throw new NetworkAuthorityException();
        _explosion.Explode(spawnContext.spawnerClientID);
    }

    
}
