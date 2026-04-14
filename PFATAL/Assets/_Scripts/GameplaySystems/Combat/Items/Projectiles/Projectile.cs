using System;
using _Scripts.Exceptions;
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
        if (!IsServer) throw new NetworkAuthorityException();
        BroadcastDespawnRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    void BroadcastDespawnRpc()
    {
        if(IsServer)
            NetworkObject.Despawn();
        OnDespawn?.Invoke();
    }
}
