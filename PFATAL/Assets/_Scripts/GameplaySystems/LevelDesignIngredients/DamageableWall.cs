using Unity.Netcode;

public class DamageableWall : DamageableObject
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        OnDie += OnWallDestroyed;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        OnDie -= OnWallDestroyed;
    }

    private void OnWallDestroyed()
    {
        if (!IsServer) return;
        DisableWallRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void DisableWallRpc()
    {
        gameObject.SetActive(false);
    }
}