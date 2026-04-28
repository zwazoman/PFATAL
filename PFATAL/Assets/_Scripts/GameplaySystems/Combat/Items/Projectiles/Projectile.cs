using System;
using _Scripts.Exceptions;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public event Action OnSpawn;
    public event Action OnDespawn;

    public NetworkVariable<SpawnContext> spawnContext;

    [SerializeField] protected GameObject visuals;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        BroadcastSpawnRpc();

        if (visuals != null && NetworkManager.LocalClientId == spawnContext.Value.spawnerClientID)
            visuals.SetActive(false);
    }

    public virtual void Despawn()
    {
        BroadcastDespawnRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    void BroadcastDespawnRpc()
    {
        OnDespawn?.Invoke();

        if (IsServer)
            NetworkObject.Despawn();
    }

    [Rpc(SendTo.Everyone)]
    void BroadcastSpawnRpc()
    {
        OnSpawn?.Invoke();
    }
}
