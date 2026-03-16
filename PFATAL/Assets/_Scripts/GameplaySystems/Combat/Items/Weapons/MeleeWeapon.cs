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

    protected virtual async void Hit()
    {
        //todo vraie animation

        canHit = false;

        await Awaitable.WaitForSecondsAsync(.3f);
        isHitting = true;
        await Awaitable.WaitForSecondsAsync(2f);
        isHitting = false;

        _hitDamageables.Clear();

        canHit = true;
    }

    protected virtual void Update()
    {
        if (isHitting)
        {
            Collider[] _hitColliders = Physics.OverlapSphere(hitSocket.position, hitSphereRadius, _hitLayerMask);

            print(_hitColliders.Length);

            foreach(Collider collider in _hitColliders)
            {
                if (collider.gameObject.TryGetComponent(out DamageableObject damageable))
                {
                    if (damageable.OwnerClientId == playerCharacter.OwnerClientId || _hitDamageables.Contains(damageable) )
                        continue;

                    print(damageable.gameObject.name);

                    _hitDamageables.Add(damageable);
                    ApplyHit(damageable);
                }
            }
        }
    }

    protected virtual void ApplyHit(DamageableObject damageable) { }


    private void OnDrawGizmos()
    {
        if (isHitting)
            Gizmos.DrawWireSphere(hitSocket.position, hitSphereRadius);
    }
}
