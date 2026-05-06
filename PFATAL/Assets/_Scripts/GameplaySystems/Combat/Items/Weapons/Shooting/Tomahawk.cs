using _Scripts.StateMachine;
using System;
using GameplaySystems.PlayerCharacter;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using state = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class Tomahawk : ProjectileWeapon
{
    public event Action OnDash;

    public event Action<Projectile> OnTomahawkShoot;

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

    int _currentAmmoCount;

    bool _dashed = false;
    public bool CanDash { get; private set; }= true;

    float _reloadTimer;

    public override void Equip()
    {
        base.Equip();

        currentDashCooldown = dashCooldown;
        _currentAmmoCount = maxAmmoAmount;
        _dashed = false;
        CanDash = true;

        playerCharacter.replicatedStateMachineCallbacks.OnStateChanged += ResetDash;

        try
        {
            hand.EquipSpecific(this);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
        //link animation events
        hand.animatorEventListener.OnGrapplePulled += DashTowardsProj;
        hand.animatorEventListener.OnTomahawkShot += SpawnProjectile;
    }

    public override void UnEquip()
    {
        base.UnEquip();

        currentDashCooldown = dashCooldown;
        
        //unlink animation events
        hand.animatorEventListener.OnGrapplePulled -= DashTowardsProj;
        hand.animatorEventListener.OnTomahawkShot -= SpawnProjectile;
    }

    public override void UseUpdate()
    {
        base.UseUpdate();
        
        //quand on reste appuyé longtemps
        if (holdDuration >= _dashHoldDuration && _currentProjectile != null && !_dashed && CanDash)
        {
            //play dash animation
            hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.tomahawk_grapple);
        }
    }

    private void Update()
    {
        //reload progressif des 3 munitions
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
        
        //quand on relache
        if (canShoot && !_dashed && _currentAmmoCount > 0)
        {
            Shoot();
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

        Vector3 dashDirection = (_currentProjectile.transform.position - playerCharacter.transform.position).normalized;

        playerCharacter.physics.SetVelocity(Vector3.zero);
        playerCharacter.physics.AddImpulse(dashDirection * _dashStrength);

        _currentProjectile.Despawn();

        CanDash = false;
        //HandleDashDelay();
    }

    void ResetDash(state previousState, state newState)
    {
        if ((previousState == state.Falling) && ((newState & state.Grounded) == state.Grounded))
            CanDash = true;
    }

    async void HandleDashDelay()
    {
        //todo link au crosshair

        CanDash = false;

        currentDashCooldown = 0;

        while(currentDashCooldown < dashCooldown)
        {
            currentDashCooldown += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }

        currentDashCooldown = dashCooldown;

        CanDash = true;
    }
    
    void Shoot()
    {
        //diminish ammo
        _currentAmmoCount--;
        OnConsumeAmmo?.Invoke(_currentAmmoCount);
        if (_currentAmmoCount == 0)
            OnAmmoEmpty?.Invoke();
        
        //play throw animation
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.tomahawk_throw);
    }

    //appelé par le callback de l'animator
    async void SpawnProjectile()
    {
        print("Spawn Projectile");
        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;
        await Shoot(context, Quaternion.Euler(-_projXOffset, 0, 0), shootSocket.position);
        OnTomahawkShoot?.Invoke(_currentProjectile);
    }
    
}