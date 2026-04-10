using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public event Action OnDespawn;

    public NetworkVariable<SpawnContext> spawnContext;

    [SerializeField] protected GameObject visuals;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (visuals != null && NetworkManager.LocalClientId == spawnContext.Value.spawnerClientID)
            visuals.SetActive(false);
    }

    public virtual void Despawn()
    {
        BroadcastDespawnRpc();
        DespawnRpc();
    }

    [Rpc(SendTo.Server)]
    void DespawnRpc()
    {
        NetworkObject.Despawn();
    }

    [Rpc(SendTo.Everyone)]
    void BroadcastDespawnRpc()
    {
        OnDespawn?.Invoke();
    }
}
