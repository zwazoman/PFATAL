using Unity.Netcode;
using UnityEngine;

public class Proj_ToxicCloud : Proj_Falling
{
    [SerializeField] float _throwStrength = 5f;
    [SerializeField] GameObject _cloudPrefab;
    private bool _hasHit = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        transform.forward = transform.forward + Vector3.up * 0.2f;

        speed = _throwStrength;
    }

    public override void Despawn()
    {
        if (IsServer && !_hasHit)
        {
            SpawnCloud();
        }
        base.Despawn();
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