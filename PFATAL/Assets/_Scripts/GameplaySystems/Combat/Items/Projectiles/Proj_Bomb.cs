using Unity.Netcode;
using UnityEngine;

public class Proj_Bomb : Projectile
{
    [Header("References")]
    [SerializeField] Rigidbody _rb;

    [Header("Settings")]
    [SerializeField] float _fuseTime = 3f;

    float _timer;
    

    private void Awake()
    {
        TryGetComponent(out  _rb);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _timer = 0f;

        Vector3 force = transform.forward * 5 + transform.up * 3;
        _rb.AddForce(force * 25, ForceMode.Impulse);
    }

    private void Update()
    {
        if (!IsSpawned)
            return;

        _timer += Time.deltaTime;
        if(_timer >= _fuseTime)
        {
            Explode();
            NetworkObject.Despawn();
        }
    }

    void Explode()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _explosionRange);
    }
}
