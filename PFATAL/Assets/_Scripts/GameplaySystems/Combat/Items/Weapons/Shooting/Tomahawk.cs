using System;
using UnityEngine;

public class Tomahawk : ProjectileWeapon
{
    public event Action OnDash;

    [Header("Tomahawk Settings")]
    [SerializeField] float _projXOffset = 15;

    [Header("Tomahawk Dash Settings")]
    [SerializeField] float _dashHoldDuration = .3f;
    [SerializeField] float _dashStrength = 10;
    [SerializeField] float _dashCooldown = 3;

    GameObject _tomahawkProj;

    bool _dashed = false;
    bool _canDash = true;

    public override void UseUpdate()
    {
        base.UseUpdate();

        if (holdDuration >= _dashHoldDuration && _tomahawkProj != null && !_dashed)
        {
            DashTowardsProj();
            StartShootDelay();
        }
    }

    public override async void StopUsing()
    {
        if(canShoot && !_dashed)
        {
            SpawnContext context = new(playerCharacter.OwnerClientId);
            Quaternion rotation = ComputeProjectileRotation() * Quaternion.Euler(-_projXOffset, 0, 0);
            _tomahawkProj = await Shoot(context, rotation);
        }

        _dashed = false;

        base.StopUsing();
    }

    void DashTowardsProj()
    {
        if (!_canDash)
            return;

        _dashed = true;

        Vector3 dashDirection = (_tomahawkProj.transform.position - playerCharacter.transform.position).normalized;

        playerCharacter.physics.AddImpulse(dashDirection * _dashStrength);

        _tomahawkProj.TryGetComponent(out Proj_Tomahawk proj);
        proj.Despawn();

        HandleDashDelay();
    }

    async void HandleDashDelay()
    {
        //todo UI 

        _canDash = false;
        await Awaitable.WaitForSecondsAsync(_dashCooldown);
        _canDash = true;
    }
}
