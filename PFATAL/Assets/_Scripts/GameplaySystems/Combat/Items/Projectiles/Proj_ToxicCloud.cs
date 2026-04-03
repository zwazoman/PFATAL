using Unity.Netcode;
using UnityEngine;

public class Proj_ToxicCloud : Projectile
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _throwStrength = 30f;
    [SerializeField] GameObject _cloudPrefab;

    private bool _hasHit = false;

    private void Awake()
    {
        TryGetComponent(out _rb);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

        _rb.isKinematic = false;

        Vector3 force = transform.forward * 5 + transform.up * 3;
        _rb.AddForce(force.normalized * _throwStrength, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || _hasHit) return;

        _hasHit = true;

        SpawnCloud();
    }

    void SpawnCloud()
    {
        GameObject obj = Instantiate(_cloudPrefab, transform.position, Quaternion.identity);

        if (obj.TryGetComponent(out NetworkObject netObj)) netObj.Spawn();

        if (obj.TryGetComponent(out ToxicCloud cloud))
        {
            cloud.Init(spawnContext.Value.spawnerClientID);
        }

        Despawn();
    }
}