using Unity.Netcode;
using UnityEngine;

public class Proj_Bomb : Projectile
{
    [Header("References")]
    [SerializeField] Rigidbody _rb;

    [Header("Settings")]
    [SerializeField] float _fuseTime = 3f;
    [SerializeField] float _explosionRange = 3f;
    [SerializeField] float _damages = 5;

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
        DamageData data = new();
        data.Amount = _damages;
        data.SourcePlayerClientID = spawnContext.askerID;

        ColliderHit hit;

        foreach(Collider coll in Physics.OverlapSphere(transform.position, _explosionRange))
        {
            if(coll.TryGetComponent(out DamageableObject damageable))
            {
                print($"{damageable.gameObject.name} was hit by a bomb !");
                data.Point = transform.position;
                data.Radius = _explosionRange;

                damageable.TakeDamage(data);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _explosionRange);
    }
}
