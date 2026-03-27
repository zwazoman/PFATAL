using Unity.Netcode;
using UnityEngine;

public class Proj_BigLaserBeam : Projectile
{
    [Header("Laser Settings")]
    [SerializeField] private float _sphereRadius = 0.5f;
    [SerializeField] private float _maxRange = 50f;
    [SerializeField] private float _maxDamage = 100f;
    [SerializeField] private LayerMask _hitLayer;

    public void Fire(Vector3 origin, Vector3 direction, float chargeRatio, ulong sourceId)
    {
        if (Physics.SphereCast(origin, _sphereRadius, direction, out RaycastHit hit, _maxRange, _hitLayer))
        {
            Debug.Log($"[Proj_BigLaserBeam] Touché : {hit.collider.name}");

            if (hit.collider.TryGetComponent(out DamageableObject damageable))
            {
                DamageData damage = new DamageData(
                    amount:               _maxDamage * chargeRatio,
                    point:                hit.point,
                    direction:            direction,
                    sourcePlayerClientID: sourceId
                );

                damageable.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log("[Proj_BigLaserBeam] Aucun hit.");
        }

        Despawn();
    }
}