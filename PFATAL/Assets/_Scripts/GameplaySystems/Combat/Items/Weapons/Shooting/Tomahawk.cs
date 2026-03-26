using UnityEngine;

public class Tomahawk : ProjectileWeapon
{

    public override void StartUsing()
    {
        if (canShoot)
        {
            SpawnContext context = new(playerCharacter.OwnerClientId);
            Shoot(context);
        }

        base.StartUsing();
    }
}
