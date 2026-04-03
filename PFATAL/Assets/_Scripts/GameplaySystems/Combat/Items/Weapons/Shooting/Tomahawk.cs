using System;
using UnityEngine;

public class Tomahawk : ProjectileWeapon
{
    public event Action OnDash;

    public event Action OnLoadAmmo;
    public event Action OnAmmoEmpty;
    public event Action OnAmmoFull;

    [Header("Tomahawk Settings")]
    [SerializeField] float _projXOffset = 15;
    [SerializeField] int _maxAmmoAmount = 3;
    [SerializeField] float _reloadTime = .7f;

    [Header("Tomahawk Dash Settings")]
    [SerializeField] float _dashHoldDuration = .3f;
    [SerializeField] float _dashStrength = 10;
    [SerializeField] float _dashCooldown = 3;

    GameObject _tomahawkProj;

    int _currentAmmoCount;

    bool _dashed = false;
    bool _canDash = true;

    float _timer;

    public override void Equip()
    {
        base.Equip();

        _currentAmmoCount = _maxAmmoAmount;
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

        if (canShoot && !_dashed && _currentAmmoCount > 0)
        {
            SpawnContext context = new(playerCharacter.OwnerClientId);
            Quaternion rotation = ComputeProjectileRotation() * Quaternion.Euler(-_projXOffset, 0, 0);
            _tomahawkProj = await Shoot(context, rotation);

            _currentAmmoCount--;
            if(_currentAmmoCount == 0)
                OnAmmoEmpty?.Invoke();
        }

        _dashed = false;
    }

    private void Update()
    {
        print(_currentAmmoCount);

        //todo fleche qui pointe vers le tomahawk actuel
        

        if (_currentAmmoCount < _maxAmmoAmount)
        {
            _timer += Time.deltaTime;

            if(_timer >= _reloadTime)
            {
                LoadAmmo();
                _timer = 0;
            }
        }
    }

    void LoadAmmo()
    {
        OnLoadAmmo?.Invoke();
        _currentAmmoCount++;
        if (_currentAmmoCount == _maxAmmoAmount)
            OnAmmoFull?.Invoke();
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
