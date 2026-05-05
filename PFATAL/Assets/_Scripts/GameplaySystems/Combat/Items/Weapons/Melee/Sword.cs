using UnityEngine;
using System;

public class Sword : MeleeWeapon
{
    public event Action OnDashCooledUp;

    public event Action OnStartCharging;
    public event Action OnStopCharging;

    [Header("Sword References")]
    [SerializeField] Animator _animator;
    [SerializeField] public SwordEventReceiver EventReceiver;

    [Header("Sword Settings")]
    [SerializeField] float _knockbackStrength = 10;

    [Header("Dash Settings")]
    [SerializeField] public float dashCooldown = 1.5f;
    [SerializeField] float _dashChargedDuration;
    [SerializeField] float _dashStrength = 7;
    [SerializeField] float _dashDmgMult = .8f;
    [SerializeField] float _dashDotThreshold = 0f;

    [HideInInspector] public float currentDashCooldown;

    bool _charged;
    bool _isAttacking;
    bool _canDash = true;
    bool _dashed;

    public override void Equip()
    {
        base.Equip();

        _animator.SetTrigger("Idle");

        _charged = false;
        _dashed = false;
        _isAttacking = false;
        isHitting = false;
        _canDash = true;

        currentDashCooldown = 0;

        EventReceiver.OnHitStart += StartHitting;
        EventReceiver.OnHitEnd += StopHitting;

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

        OnStopCharging?.Invoke();

        currentDashCooldown = dashCooldown;

        EventReceiver.OnHitStart -= StartHitting;
        EventReceiver.OnHitEnd -= StopHitting;
    }

    protected override void ApplyHit(DamageableObject damageable, ulong attackerId)
    {
        base.ApplyHit(damageable, attackerId);

        DamageData data = new();
        data.Point = hitSocket.position;
        data.Direction = hitSocket.transform.forward;
        data.SourcePos = playerCharacter.transform.position;

        if (_dashed)
            data.Amount = damageAmount * _dashDmgMult;
        else
            data.Amount = damageAmount;

        data.Radius = hitSphereRadius;
        data.SourcePlayerClientID = playerCharacter.OwnerClientId;
        data.KnockbackForce = playerCharacter.transform.forward * _knockbackStrength;
        data.WeaponID = ItemID;

        Summoner.Instance.ApplyDamageRpc(damageable, data);
    }

    public override void UseUpdate()
    {
        base.UseUpdate();

        if(holdDuration >= _dashChargedDuration && !_charged && !_isAttacking && _canDash)
        {
            OnStartCharging?.Invoke();

            _animator.SetTrigger("Charged");
            _charged = true;
        }
    }

    public override void StopUsing()
    {
        base.StopUsing();

        if (_isAttacking)
            return;

        if(_charged)
        {
            _animator.SetTrigger("Dash");
            Dash();

            OnStopCharging?.Invoke();
            _charged = false;
        }
        else
            _animator.SetTrigger("Hit");

        _isAttacking = true;
    }

    void Dash()
    {
        float dot = Vector3.Dot(playerCharacter.playerCamera.transform.forward, playerCharacter.physics.Velocity.normalized);

        if (dot <= _dashDotThreshold)
            playerCharacter.physics.SetVelocity(Vector3.zero);

        playerCharacter.physics.AddImpulse(playerCharacter.playerCamera.transform.forward * _dashStrength);

        HandleDashDelay();
    }

    async void HandleDashDelay()
    {
        _canDash = false;

        while(currentDashCooldown < dashCooldown)
        {
            currentDashCooldown += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }

        currentDashCooldown = 0;

        OnDashCooledUp?.Invoke();
        _canDash = true;
    }

    /// <summary>
    /// callback d'animation
    /// </summary>
    public void StartHitting(bool dashed)
    {
        _dashed = dashed;
        isHitting = true;
    }

    /// <summary>
    /// callback d'animation
    /// </summary>
    public void StopHitting() 
    {
        isHitting = false;
        _isAttacking = false;
    }

    
}
