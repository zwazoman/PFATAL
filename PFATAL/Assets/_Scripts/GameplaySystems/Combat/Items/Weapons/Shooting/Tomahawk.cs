using UnityEngine;

public class Tomahawk : ProjectileWeapon
{
    public override void StartUsing()
    {
        base.StartUsing();

        if (canShoot)
        {
            SpawnContext context = new(_playerCharacter.OwnerClientId);
            Shoot(context);
        }
    }
}
