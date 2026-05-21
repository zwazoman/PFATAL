using Unity.Netcode;
using UnityEngine;

public class Proj_NetworkVisual : NetworkBehaviour
{
    [SerializeField] GameObject visualPrefab;

    public override void OnNetworkSpawn()
    {
        var go = Instantiate(visualPrefab, transform.position, transform.rotation);
        if (go.TryGetComponent(out Proj_Visual visual))
        {
            visual.context = GetComponent<Projectile>().spawnContext.Value;
            visual.trueProjectile = GetComponent<Projectile>();
        }

        if (!IsServer)
        {
            gameObject.SetActive(false);
        }
    }
}