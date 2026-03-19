using UnityEngine;

public class Tomahawk : ProjectileWeapon
{

    public override void StopUsing()
    {
        SpawnContext context = new(playerCharacter.OwnerClientId);
        Shoot(context);

        base.StopUsing();
    }
}
