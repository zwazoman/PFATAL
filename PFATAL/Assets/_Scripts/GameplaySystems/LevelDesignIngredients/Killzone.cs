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
            data.SourcePos = transform.position;
            data.Point = transform.position;
            data.KnockbackForce = Vector3.up * 10;
            damageable.TakeDamage(data);
        }
    }
}
