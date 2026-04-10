using System;
using System.Collections.Generic;
using _scripts.PlayerCharacter;
using UnityEngine;

public class PlayerCharacterSpawner : MonoBehaviour
{
    public static PlayerCharacterSpawner Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    [Header("Asset references")]
    [SerializeField] GameObject _characterPrefab;
    
    [Header("Scene references")]
    [SerializeField] List<Transform> _spawnSockets;

    [Header("SpawnPoint Settings")]
    [SerializeField] float _playerDetectionRadius = 5f;
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
        SpawnContext context = new(0);

        while (!Summoner.Instance.IsSpawned)
            await Awaitable.NextFrameAsync();

        GameObject player = await Summoner.Instance.SpawnObject(_characterPrefab, spawnSocket.position, spawnSocket.rotation,true, context, ownerClientID);
        PlayerCharacter playerchara = player.GetComponent<PlayerCharacter>();

        GameManager.Instance.SetLocalPlayer(playerchara, ownerClientID);

        return playerchara;
    }

    public void ReSpawnPlayer(PlayerCharacter player)
    {
        Transform spawnSocket = SelectSpawnSocket();

        player.physics.SetPosition(spawnSocket.position);
        player.transform.rotation = spawnSocket.rotation;
    }

    Transform SelectSpawnSocket()
    {
        if (_spawnSockets.Count == 0)
            return transform;

        List<Transform> availableSockets = new();
        
        //enumerate all spawn sockets
        foreach(Transform spawnSocket in _spawnSockets)
        {
            //check if a player is nearby
            Collider[] players = Physics.OverlapSphere(spawnSocket.position, _playerDetectionRadius, _playerDetectionMask);
            if (players.Length > 0)
                continue;

            availableSockets.Add(spawnSocket);
        }
        
        if (availableSockets.Count == 0)
            return _spawnSockets.PickRandom();

        return availableSockets.PickRandom();
    }
}
