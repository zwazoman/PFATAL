using Unity.Netcode;
using UnityEngine;

public class Killzone : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("trigger");

        if (!IsServer) 
            return;

        if (other.TryGetComponent(out DamageableObject damageable))
        {
            print("Killzone Hit");

            DamageData data = new();
            data.Amount = damageable.MaxHP;
            damageable.TakeDamage(data);
        }
    }
}
