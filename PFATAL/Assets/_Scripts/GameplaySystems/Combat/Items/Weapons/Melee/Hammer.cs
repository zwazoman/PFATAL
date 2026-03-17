using UnityEngine;

public class Hammer : MeleeWeapon
{
    [Header("Hammer Settings")]
    [SerializeField] float _holdDuration;
    [SerializeField] float _dashStrength = 15;

    protected override void ApplyHit(DamageableObject damageable)
    {
        base.ApplyHit(damageable);

        print("hit hammer");

        DamageData data = new();
        data.Point = hitSocket.position;
        data.Direction = transform.forward;
        data.Amount = damageAmount;
        data.Radius = hitSphereRadius;
        data.SourcePlayerClientID = playerCharacter.OwnerClientId;
        data.KnockbackForce = playerCharacter.transform.forward * 5;
    }

    public override void StopUsing()
    {
        if(holdDuration > _holdDuration)
        {
            playerCharacter.physics.AddImpulse(playerCharacter.playerCamera.transform.forward * _dashStrength);
        }

        Hit();
        base.StopUsing();
    }

    protected override void Update()
    {
        base.Update();
    }
}
