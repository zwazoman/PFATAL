using Unity.Netcode;
using UnityEngine;

public class Killzone : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) 
            return;

        if (other.TryGetComponent(out DamageableObject damageable))
        {
            DamageData data = new();
            data.Amount = damageable.MaxHP;
            damageable.TakeDamage(data);
        }
    }
}
