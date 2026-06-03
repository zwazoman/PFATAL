using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class NetworkPool : NetworkBehaviour, INetworkPrefabInstanceHandler
{
    [SerializeField] GameObject _prefab;
    [SerializeField] int _poolSize;

    Queue<GameObject> _pool = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.PrefabHandler.AddHandler(_prefab, this);
        InitPool();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
        NetworkManager.PrefabHandler.RemoveHandler(_prefab);
    }

    public void InitPool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject newObject = Instantiate(_prefab, transform);
            _pool.Enqueue(newObject);
            newObject.SetActive(false);
        }
    }

    public void Destroy(NetworkObject networkObject)
    {
        if (this == null) return;

        networkObject.gameObject.SetActive(false);
        networkObject.transform.SetPositionAndRotation(transform.position, transform.rotation);

        _pool.Enqueue(networkObject.gameObject);
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        GameObject newObject = _pool.Dequeue();
        newObject.transform.SetPositionAndRotation(position, rotation);
        newObject.SetActive(true);

        NetworkObject networkBhv = newObject.GetComponent<NetworkObject>();
        return networkBhv;
    }
}