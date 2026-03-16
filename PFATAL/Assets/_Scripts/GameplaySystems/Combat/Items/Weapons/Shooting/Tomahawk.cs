using UnityEngine;

public class Tomahawk : ProjectileWeapon
{
    [Header("Tomahawk Settings")]

    [SerializeField] float _holdActionDuration = .5f;

    [SerializeField] float _afterDashDelay = 2;
    [SerializeField] float _dashStrength = 10;

    GameObject _currentProjectile;
    bool _canDash;


    public override void UseUpdate()
    {
        base.UseUpdate();

        if (_currentProjectile != null && holdDuration > _holdActionDuration && _canDash)
        {
            playerCharacter.physics.AddImpulse((_currentProjectile.transform.position - playerCharacter.physics.Rb.position).normalized * _dashStrength);
            _canDash = false;
        }
    }

    public override async void StopUsing()
    {
        float tmpDuration = holdDuration;

        base.StopUsing();

        _canDash = true;

        if (tmpDuration <= _holdActionDuration && canShoot)
        {
            SpawnContext context = new(playerCharacter.OwnerClientId);
            _currentProjectile = await Shoot(context);
        }
    }
}
