using Unity.Netcode;
using UnityEngine;

//todo : refaire sans le rigidbody, avec la physique de proj_falling
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

        Transform cameraTransform= GameManager.Instance.GetPlayerCharacter(spawnContext.Value.spawnerClientID).playerCamera.transform;
        Vector3 force =  cameraTransform.forward * 5 + cameraTransform.up * 3;
        _rb.AddForce(force.normalized * _throwStrength, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || _hasHit) return;

        _hasHit = true;

        print(collision.gameObject.name);

        SpawnCloud();
        Despawn();
    }

    /// <summary>
    /// appelé sur le server seulement
    /// </summary>
    void SpawnCloud()
    {
        //todo : POOL !!!
        GameObject obj = Instantiate(_cloudPrefab, transform.position, Quaternion.identity);
        if (obj.TryGetComponent(out ToxicCloud cloud))
            cloud.Init(spawnContext.Value.spawnerClientID);
        
        if (obj.TryGetComponent(out NetworkObject netObj)) netObj.Spawn();
    }
    
}