using UnityEngine;

public class Tomahawk : ProjectileWeapon
{

    public override void StopUsing()
    {
        if (canShoot)
        {
            SpawnContext context = new(playerCharacter.OwnerClientId);
            Shoot(context);
        }

        base.StopUsing();
    }
}
