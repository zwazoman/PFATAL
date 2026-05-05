using UnityEngine;

public class Proj_BigLaserBeam : Projectile
{
    [Header("Laser Settings")]
    [SerializeField] private float _sphereRadius = 0.5f;
    [SerializeField] private float _maxRange = 50f;
    [SerializeField] private float _maxDamage = 100f;
    [SerializeField] private LayerMask _hitLayer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        ulong sourceId = (ulong)spawnContext.Value.spawnerClientID;
        float chargeRatio = spawnContext.Value.floatData;
        int sourceWeaponID = (int)spawnContext.Value.floatData2;

        if (Physics.SphereCast(transform.position, _sphereRadius, transform.forward, out RaycastHit hit, _maxRange, _hitLayer))
        {
            Debug.Log($"[Proj_BigLaserBeam] Touché : {hit.collider.name}");

            if (hit.collider.TryGetComponent(out DamageableObject damageable))
            {
                DamageData damage = new DamageData(
                    amount:               _maxDamage * chargeRatio,
                    point:                hit.point,
                    sourcePos:            GameManager.Instance.GetPlayerCharacter(spawnContext.Value.spawnerClientID).transform.position,
                    direction:            transform.forward,
                    sourcePlayerClientID: sourceId,
                    weaponID:             sourceWeaponID
                );

                damageable.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log("[Proj_BigLaserBeam] Aucun hit.");
        }

        DespawnNextFrame();
    }

    private async void DespawnNextFrame()
    {
        await Awaitable.WaitForSecondsAsync(1f);
        Despawn();
    }
}