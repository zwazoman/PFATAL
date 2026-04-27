using Unity.Netcode;
using UnityEngine;

public class Killzone : NetworkBehaviour
{
    [SerializeField] float _damages = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) 
            return;

        if (other.TryGetComponent(out DamageableObject damageable))
        {
            DamageData data = new();
            data.Amount = _damages;
            damageable.TakeDamage(data);
        }
    }
}
