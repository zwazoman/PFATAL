using _Scripts.StateMachine;
using System;
using GameplaySystems.PlayerCharacter;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using state = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class Tomahawk : ProjectileWeapon
{
    public event Action OnStartGrapple;

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
    [SerializeField] float _dashStrength = 10;
    [SerializeField] float _dashDelay = .2f;

    int _currentAmmoCount;

    bool _isShooting;
    bool _canGrapple;
    bool _canReload;

    float _reloadTimer;
    private Vector3 _dashDirection;

    public override void Equip()
    {
        base.Equip();

        canShoot = true;
        _isShooting = false;
        _canGrapple = true;
        _canReload = true;

        _currentAmmoCount = maxAmmoAmount;

        try
        {
            hand.EquipSpecific(this);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        playerCharacter.replicatedStateMachineCallbacks.OnStateChanged += StateChanged_Callback;

        //link animation events
        hand.animatorEventListener.OnGrapplePulled += DashTowardsProj;
        hand.animatorEventListener.OnTomahawkShot += SpawnProjectile;
    }

    public override void UnEquip()
    {
        base.UnEquip();
        
        //unlink animation events
        hand.animatorEventListener.OnGrapplePulled -= DashTowardsProj;
        hand.animatorEventListener.OnTomahawkShot -= SpawnProjectile;
    }

    void StateChanged_Callback(state previousState, state newState)
    {
        if((newState & state.Grounded) == state.Grounded)
        {
            _canReload = true;
        }
    }

    private void Update()
    {
        if (_currentAmmoCount < maxAmmoAmount && _canReload)
        {
            _reloadTimer += Time.deltaTime;

            if (_reloadTimer >= _reloadTime)
            {
                LoadSingleAmmo();
                _reloadTimer = 0;
            }
        }
    }

    public override void StartUsing()
    {
        base.StartUsing();

        if(canShoot && _currentProjectile == null && _currentAmmoCount > 0 && !_isShooting)
        {
            Shoot();
        }
        else if (_canGrapple && _currentProjectile != null && !_isShooting)
        {
            _canGrapple = false;

            //play dash animation
            OnStartGrapple?.Invoke();

            _dashDirection = (_currentProjectile.transform.position - playerCharacter.transform.position).normalized;
            hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.tomahawk_grapple);
        }
    }

    void LoadSingleAmmo()
    {
        _currentAmmoCount++;

        OnLoadAmmo?.Invoke(_currentAmmoCount);

        if (_currentAmmoCount == maxAmmoAmount)
            OnAmmoFull?.Invoke();
    }

    void DashTowardsProj()
    {
        StartShootDelay();

        _canReload = false;

        playerCharacter.physics.SetVelocity(Vector3.zero);
        playerCharacter.physics.AddImpulse(_dashDirection * _dashStrength);

        if (_currentProjectile != null)
        {
            Proj_Tomahawk tomahawk = _currentProjectile as Proj_Tomahawk;
            tomahawk.Explode();
            tomahawk.Despawn();
        }
    }

    void Shoot()
    {
        _isShooting = true;
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
        _isShooting = false;

        print("Spawn Projectile");
        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;
        await Shoot(context, Quaternion.Euler(-_projXOffset, 0, 0), playerCharacter.playerCamera.transform.position);
        StartDashDelay();
        OnTomahawkShoot?.Invoke(_currentProjectile);

        _currentProjectile.OnDespawn += TomahawkDespawn_Callback;
    }

    void TomahawkDespawn_Callback()
    {
        StartShootDelay();

        if(_currentProjectile != null)
            _currentProjectile.OnDespawn -= TomahawkDespawn_Callback;
    }

    async void StartDashDelay()
    {
        await Awaitable.WaitForSecondsAsync(_dashDelay);

        _canGrapple = true;
    }
}