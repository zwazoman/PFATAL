using _scripts.PlayerCharacter;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeWeapon : Item
{
    [Header("References")]
    [SerializeField] protected Transform hitSocket;

    [Header("Weapon Settings")]
    [SerializeField] protected float hitSphereRadius = 1f;
    [SerializeField] protected float damageAmount = 1f;
    [SerializeField] LayerMask _hitLayerMask;

    protected bool canHit;
    protected bool isHitting;

    List<DamageableObject> _hitDamageables = new();

    protected virtual void Update()
    {
        if (isHitting)
        {
            Collider[] _hitColliders = Physics.OverlapSphere(hitSocket.position, hitSphereRadius, _hitLayerMask);

            foreach(Collider collider in _hitColliders)
            {
                if (collider.gameObject.TryGetComponent(out DamageableObject damageable))
                {
                    if ((damageable.OwnerClientId == playerCharacter.OwnerClientId && damageable.isPlayer) || _hitDamageables.Contains(damageable) )
                        continue;

                    _hitDamageables.Add(damageable);
                    ApplyHitRpc(damageable);
                }
            }
        }
        else if(_hitDamageables.Count > 0) 
            _hitDamageables.Clear();
    }

    protected virtual void ApplyHitRpc(DamageableObject damageable) { }


    private void OnDrawGizmos()
    {
        if (isHitting)
            Gizmos.DrawWireSphere(hitSocket.position, hitSphereRadius);
    }
}
