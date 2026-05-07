using System.Collections.Generic;
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
    protected bool hitboxIsActive;

    List<DamageableObject> _hitDamageables = new();

    protected virtual void Update()
    {
        if (hitboxIsActive)
        {
            Collider[] _hitColliders = Physics.OverlapSphere(hitSocket.position, hitSphereRadius, _hitLayerMask);

            foreach(Collider collider in _hitColliders)
            {
                if (collider.gameObject.TryGetComponent(out DamageableObject damageable))
                {
                    if ((damageable.OwnerClientId == playerCharacter.OwnerClientId && damageable.isPlayer) 
                        || _hitDamageables.Contains(damageable) )
                        continue;

                    _hitDamageables.Add(damageable);
                    ApplyHit(damageable, playerCharacter.OwnerClientId);
                }
            }
        }
        else if(_hitDamageables.Count > 0) 
            _hitDamageables.Clear();
    }

    /// <summary>
    /// appelé quand l'arme est active (isHitting) et touche un ennemi
    /// </summary>
    /// <param name="damageable"></param>
    /// <param name="attackerId"></param>
    protected virtual void ApplyHit(DamageableObject damageable, ulong attackerId) { }
    
    private void OnDrawGizmos()
    {
        if (hitboxIsActive)
            Gizmos.DrawWireSphere(hitSocket.position, hitSphereRadius);
    }
}
