using System;
using UnityEngine;

public class Tomahawk : ProjectileWeapon
{
    public event Action OnDash;

    public event Action<GameObject> OnTomahawkShoot;

    public event Action<int> OnLoadAmmo;
    public event Action<int> OnConsumeAmmo;

    public event Action OnAmmoEmpty;
    public event Action OnAmmoFull;

    [Header("Tomahawk Settings")]
    [SerializeField] public int maxAmmoAmount = 3;
    [SerializeField] float _projXOffset = 15;
    [SerializeField] float _reloadTime = .7f;

    [Header("Tomahawk Dash Settings")]
    [SerializeField] float _dashHoldDuration = .3f;
    [SerializeField] float _dashStrength = 10;
    [SerializeField] public float dashCooldown = 3;

    [HideInInspector] public float currentDashCooldown;


    GameObject _tomahawkProj;

    int _currentAmmoCount;

    bool _dashed = false;
    bool _canDash = true;

    float _reloadTimer;

    public override void Equip()
    {
        base.Equip();

        currentDashCooldown = 0;
        _currentAmmoCount = maxAmmoAmount;
        _dashed = false;
        _canDash = true;

        _tomahawkProj = null;

        try
        {
            hand.EquipSpecific(this);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public override void UnEquip()
    {
        base.UnEquip();

        currentDashCooldown = 0;
    }

    public override void UseUpdate()
    {
        base.UseUpdate();

        if (holdDuration >= _dashHoldDuration && _tomahawkProj != null && !_dashed && _canDash)
        {
            DashTowardsProj();
        }
    }

    private void Update()
    {
        if (_currentAmmoCount < maxAmmoAmount)
        {
            _reloadTimer += Time.deltaTime;

            if (_reloadTimer >= _reloadTime)
            {
                LoadAmmo();
                _reloadTimer = 0;
            }
        }
    }

    public override void StopUsing()
    {
        base.StopUsing();

        if (canShoot && !_dashed && _currentAmmoCount > 0)
        {
            HandleShoot();
        }

        _dashed = false;
    }

    void LoadAmmo()
    {
        _currentAmmoCount++;

        OnLoadAmmo?.Invoke(_currentAmmoCount);

        if (_currentAmmoCount == maxAmmoAmount)
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

        currentDashCooldown = 0;

        while(currentDashCooldown < dashCooldown)
        {
            currentDashCooldown += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }

        currentDashCooldown = dashCooldown;

        _canDash = true;
    }

    async void HandleShoot()
    {
        _currentAmmoCount--;
        OnConsumeAmmo?.Invoke(_currentAmmoCount);
        if (_currentAmmoCount == 0)
            OnAmmoEmpty?.Invoke();

        SpawnContext context = new(playerCharacter.OwnerClientId);
        Quaternion rotation = ComputeProjectileRotation() * Quaternion.Euler(-_projXOffset, 0, 0);
        _tomahawkProj = await Shoot(context, rotation);

        OnTomahawkShoot?.Invoke(_tomahawkProj);
    }
}
