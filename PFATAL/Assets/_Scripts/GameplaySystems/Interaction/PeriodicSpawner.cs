using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using Unity.VisualScripting;

public class PeriodicSpawner : MonoBehaviour
{
    public event Action OnSpawn;
    public event Action OnStartSpawnDelay;

    [Header("References")]
    [SerializeField] Transform _spawnSocket;
    [SerializeField] List<GameObject> _pickupPrefabsPool;

    [Header("Settings")]
    [SerializeField] float _minSpawnDelay = 3;
    [SerializeField] float _maxSpawnDelay = 3;

    Pickup _currentPickup;

    private void Start()
    {
        if(NetworkManager.Singleton.IsServer)
            StartSpawning();
    }

    async void StartSpawning()
    {
        OnStartSpawnDelay?.Invoke();

        if(_currentPickup != null)
        {
            _currentPickup.OnPickup -= StartSpawning;
            _currentPickup = null;
        }

        await Awaitable.WaitForSecondsAsync(UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay));

        SpawnPickup(_pickupPrefabsPool.PickRandom());
    }

    async void SpawnPickup(GameObject pickupPrefab)
    {
        OnSpawn?.Invoke();

        GameObject pickupObject = await Summoner.Instance.SpawnObject(pickupPrefab.name, _spawnSocket.position, _spawnSocket.rotation);
        if (pickupObject.TryGetComponent(out _currentPickup))
        {
            _currentPickup.OnPickup += StartSpawning;
        }
    }
}
