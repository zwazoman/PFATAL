using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Netcode;

public class PeriodicSpawner : NetworkBehaviour
{
    public event Action OnServerSpawn;

    [Header("References")]
    [SerializeField] Transform _spawnSocket;
    [SerializeField] List<GameObject> _pickupPrefabsPool;

    [Header("Settings")]
    [SerializeField] float _minSpawnDelay = 3;
    [SerializeField] float _maxSpawnDelay = 3;

    Pickup _currentPickup;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            StartSpawning();
    }

    async void StartSpawning()
    {
        if(_currentPickup != null)
        {
            _currentPickup.OnOnlinePickup -= StartSpawning;
            //Debug.Log("unlink pickup event");
            _currentPickup = null;
        }

        await Awaitable.WaitForSecondsAsync(UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay));

        SpawnPickup(_pickupPrefabsPool.PickRandom());
    }

    async void SpawnPickup(GameObject pickupPrefab)
    {
        OnServerSpawn?.Invoke();
        
        GameObject pickupObject = await Summoner.Instance.SpawnObject(pickupPrefab, _spawnSocket.position, _spawnSocket.rotation,true);
        if (pickupObject.TryGetComponent(out _currentPickup))
        {
            _currentPickup.OnOnlinePickup += StartSpawning;
        }
    }
}
