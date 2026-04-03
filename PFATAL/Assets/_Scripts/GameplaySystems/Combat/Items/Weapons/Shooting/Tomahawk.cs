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

    public override void Equip()
    {
        base.Equip();

        _dashed = false;
        _canDash = true;

        _tomahawkProj = null;
    }

    public override void UseUpdate()
    {
        base.UseUpdate();

        if (holdDuration >= _dashHoldDuration && _tomahawkProj != null && !_dashed && _canDash)
        {
            DashTowardsProj();
        }
    }

    public override async void StopUsing()
    {
        base.StopUsing();

        if (canShoot && !_dashed)
        {
            SpawnContext context = new(playerCharacter.OwnerClientId);
            Quaternion rotation = ComputeProjectileRotation() * Quaternion.Euler(-_projXOffset, 0, 0);
            _tomahawkProj = await Shoot(context, rotation);
        }

        _dashed = false;
    }

    void DashTowardsProj()
    {
        _dashed = true;

        OnDash?.Invoke();

        Vector3 dashDirection = (_tomahawkProj.transform.position - playerCharacter.transform.position).normalized;

        playerCharacter.physics.SetVelocity(Vector3.zero);
        playerCharacter.physics.AddImpulse(dashDirection * _dashStrength);

        _tomahawkProj.TryGetComponent(out Proj_Tomahawk proj);
        proj.Despawn();

        HandleDashDelay();
    }

    async void HandleDashDelay()
    {
        //todo link au crosshair

        _canDash = false;
        await Awaitable.WaitForSecondsAsync(_dashCooldown);
        _canDash = true;
    }
}
