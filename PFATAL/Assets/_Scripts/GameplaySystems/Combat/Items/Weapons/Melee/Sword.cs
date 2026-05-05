using UnityEngine;
using System;

public class Sword : MeleeWeapon
{
    public event Action OnDashCooledUp;
    public event Action OnStartCharging;
    public event Action OnDashStarted;
    public event Action OnSmallAttackStarted;
    
    [Header("Sword References")]
    [SerializeField] private SwordEventReceiver _animationEventReceiver;

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
    bool _isDashing;
    bool _canDash = true;

    public override void Equip()
    {
        base.Equip();


        _charged = false;
        _isAttacking = false;
        _isDashing = false;
        hitboxIsActive = false;
        _canDash = true;

        currentDashCooldown = 0;

        try
        {
            hand.EquipSpecific(this);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        hand.animatorEventListener.OnSwordHitboxActivated += EnableHitbox;
        hand.animatorEventListener.OnSwordHitboxDeactivated += DisableHitBox;
    }

    public override void UnEquip()
    {
        base.UnEquip();
        hand.animatorEventListener.OnSwordHitboxActivated -= EnableHitbox;
        hand.animatorEventListener.OnSwordHitboxDeactivated -= DisableHitBox;
        currentDashCooldown = dashCooldown;
    }

    protected override void ApplyHit(DamageableObject damageable, ulong attackerId)
    {
        base.ApplyHit(damageable, attackerId);

        DamageData data = new();
        data.Point = hitSocket.position;
        data.Direction = hitSocket.transform.forward;
        data.SourcePos = playerCharacter.transform.position;
        data.Amount = damageAmount * (_isDashing ? 1f :_dashDmgMult);
        data.Radius = hitSphereRadius;
        data.SourcePlayerClientID = playerCharacter.OwnerClientId;
        data.KnockbackForce = playerCharacter.transform.forward * _knockbackStrength;
        data.WeaponID = ItemID;

        Summoner.Instance.ApplyDamageRpc(damageable, data);
    }

    public override void UseUpdate()
    {
        base.UseUpdate();

        //quand on reste appuyé longtemps sur la touche
        if(holdDuration >= _dashChargedDuration && !_charged && !_isAttacking && _canDash)
        {
            OnStartCharging?.Invoke();
            _charged = true;
        }
    }

    public override void StopUsing()
    {
        base.StopUsing();

        if (_isAttacking)
            return;

        //quand on relache alors qu'on avait appuyé longtemps sur le bouton
        if(_charged)
        {
            Dash();
            OnDashStarted?.Invoke();
            _charged = false;
        }
        //quand on relache après avoir appuyé peu longtemps
        else
        {
            OnSmallAttackStarted?.Invoke();
        }

        _isAttacking = true;
    }

    
    void Dash()
    {
        _isDashing = true;
        
        //todo : meilleur calcul et enlever la condition
        float dot = Vector3.Dot(playerCharacter.playerCamera.transform.forward, playerCharacter.physics.Velocity.normalized);
        if (dot <= _dashDotThreshold)
            playerCharacter.physics.SetVelocity(Vector3.zero);

        playerCharacter.physics.AddImpulse(playerCharacter.playerCamera.transform.forward * _dashStrength);

        WaitForDashToCoolDown();
    }
    async void WaitForDashToCoolDown()
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
        _isDashing = false;
    }

    
    /// <summary>
    /// callback d'animation
    /// </summary>
    public void EnableHitbox()
    {
        hitboxIsActive = true;
    }

    /// <summary>
    /// callback d'animation
    /// </summary>
    public void DisableHitBox()
    {
        hitboxIsActive = false;
        _isAttacking = false;
    }

    
}
