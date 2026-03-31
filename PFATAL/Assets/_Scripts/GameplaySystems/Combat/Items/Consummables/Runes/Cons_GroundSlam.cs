using UnityEngine;

public class Cons_GroundSlam : Consummable
{
    [SerializeField] private float _downForce = 20f;

    public override void StartUsing()
    {
        base.StartUsing();

        if (TryGetComponent(out DamageableObject hitobject))
        {
            DamageData damageData = new DamageData
            {
                Amount = 0,
                SourcePlayerClientID = playerCharacter.OwnerClientId,
                Point = hitobject.transform.position,
                Direction = -transform.up,
                KnockbackForce = -transform.up + new Vector3(0, -_downForce, 0),
                Radius = 0
            };

            hitobject.TakeDamage(damageData);
        }
    }
}