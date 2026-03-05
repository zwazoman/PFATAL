using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _scripts.PlayerCharacter;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] GameObject _playerPrefab;
    [SerializeField] List<Transform> _spawnSockets;

    [Header("SpawnPoint Settings")]
    [SerializeField] float _playerDetectionRadius = 10f;
    [SerializeField] LayerMask _playerDetectionMask;

    /// <summary>
    /// Doit être appelé par le server. le host récupère les personnages spawnés de son coté
    /// pour faire fonctionner les rêgles du jeu en les observant.
    /// </summary>
    /// <param name="ownerClientID"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Awaitable<PlayerCharacter> SpawnInitialPlayerCharacter(ulong ownerClientID)
    {
        Transform spawnSocket = SelectSpawnSocket();
        SpawnContext context = new(ownerClientID);

        GameObject player = await Summoner.Instance.SpawnObject(_playerPrefab, spawnSocket.position, spawnSocket.rotation, context, true);
        return player.GetComponent<PlayerCharacter>();
    }

    public void SpawnPlayer(PlayerCharacter player)
    {
        Transform spawnSocket = SelectSpawnSocket();

        player.physics.SetPosition(spawnSocket.position);
        player.transform.rotation = spawnSocket.rotation;
    }

    Transform SelectSpawnSocket()
    {
        if (_spawnSockets.Count == 0)
            return transform;

        Transform selectedSpawnSocket = null;

        foreach(Transform spawnSocket in _spawnSockets)
        {
            Collider[] players = Physics.OverlapSphere(spawnSocket.position, _playerDetectionRadius, _playerDetectionMask);
            if (players.Length > 0)
                continue;

            selectedSpawnSocket = spawnSocket;
        }

        if (selectedSpawnSocket == null)
            return _spawnSockets.PickRandom();

        return selectedSpawnSocket;
    }
}
