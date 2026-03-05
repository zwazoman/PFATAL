using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public SpawnContext spawnContext;

    public void Destroy()
    {
        NetworkObject.Despawn();
    }
}
