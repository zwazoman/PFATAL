using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public NetworkVariable<SpawnContext> spawnContext;

    /// <summary>
    /// appelé dans le summoner - s'appelle apres le spawn de l'objet et le setup du context
    /// </summary>
    public virtual void OnSpawn() { }

    protected virtual void Despawn()
    {
        DespawnRpc();
    }

    [Rpc(SendTo.Server)]
    void DespawnRpc()
    {
        NetworkObject.Despawn();
    }
}
