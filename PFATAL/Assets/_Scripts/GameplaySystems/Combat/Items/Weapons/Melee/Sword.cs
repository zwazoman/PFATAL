using _Scripts.Extensions;
using GameplaySystems.PlayerCharacter;
using System;
using UnityEngine;

public class Sword : MeleeWeapon
{
    public event Action OnDashCooledUp;
    public event Action OnStartCharging;
    public event Action OnDashStarted;
    public event Action OnSmallAttackStarted;
    public event Action OnAttackEnded;
    
    [Header("Sword Settings")]
    [SerializeField] float _knockbackStrength = 10;

    [Header("Dash Settings")]
    [SerializeField] public float dashCooldown = 1.5f;
    [SerializeField] float _dashChargedDuration;
    [SerializeField] float _dashStrength = 7;
    [SerializeField] float _dashDmgMult = .8f;
    [SerializeField] float _dashDotThreshold = 0f;

    [HideInInspector] public float currentDashCooldown;
    // [SerializeField] public TrailRenderer _trailRenderer;
    // [SerializeField] public Transform _trailRendererSocket;

    bool _charged;
    bool _isAttacking;
    public bool isDashing;
    bool _canDash = true;
    
    public override void Equip()
    {
        base.Equip();


        _charged = false;
        _isAttacking = false;
        isDashing = false;
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
        hand.animatorEventListener.OnAnimationFinished += AllowNextAttack;
    }

    public override void UnEquip()
    {
        base.UnEquip();
        hand.animatorEventListener.OnSwordHitboxActivated -= EnableHitbox;
        hand.animatorEventListener.OnSwordHitboxDeactivated -= DisableHitBox;
        hand.animatorEventListener.OnAnimationFinished -= AllowNextAttack;
        
        currentDashCooldown = dashCooldown;
    }

    protected override void ApplyHit(DamageableObject damageable, ulong attackerId)
    {
        base.ApplyHit(damageable, attackerId);

        DamageData data = new();
        data.Point = hitSocket.position;
        data.Direction = hitSocket.transform.forward;
        data.SourcePos = playerCharacter.transform.position;
        data.Amount = damageAmount * (isDashing ? 1f : _dashDmgMult);
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
            //event & anim charge de l'épée
            hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.sword_charge_idle);
            print("sword_charge_idle");
            OnStartCharging?.Invoke();
            _charged = true;
        }
    }

    /// <summary>
    /// (appelé à la fin des animations d'attaque)
    /// </summary>
    void AllowNextAttack()
    {
        print("Attack ended.");
        OnAttackEnded?.Invoke();
        _isAttacking = false;

        if (isDashing)
        {
            isDashing = false;
            StartDashCooldown();
        }
    }

    public override void StopUsing()
    {
        base.StopUsing();

        print("is attacking : "+_isAttacking);
        if (_isAttacking)
            return;
        print("charged : "+_charged);
        
        //quand on relache alors qu'on avait appuyé longtemps sur le bouton
        if(_charged)
        {
            _charged = false;
            Dash();
        }
        //quand on relache après avoir appuyé peu longtemps
        else
        {
            hand.visuals.PlaySwordAttackAnimation();
            OnSmallAttackStarted?.Invoke();
        }

        _isAttacking = true;
    }

    
    void Dash()
    {
        isDashing = true;
        //event & anim
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.sword_charge_release);
        hand.playerCharacter.visuals.PlaySwordDashAnimationVFX();
        OnDashStarted?.Invoke();
        
        //changelent de velocité
        //todo : meilleur calcul et enlever la condition
        float dot = Vector3.Dot(playerCharacter.playerCamera.transform.forward, playerCharacter.physics.Velocity.normalized);
        if (dot <= _dashDotThreshold)
            playerCharacter.physics.SetVelocity(Vector3.zero);

        if (playerCharacter.stateMachine.currentState != playerCharacter.stateMachine.s_Frozen)
            playerCharacter.physics.AddImpulse(playerCharacter.playerCamera.transform.forward * _dashStrength);
    }
    async void StartDashCooldown()
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
        isDashing = false;
        _isAttacking = false;
    }

    
    /// <summary>
    /// callback d'animation
    /// </summary>
    /// <param name="animationIndex">
    /// 0 -> small attack 0,  
    /// 1 -> small attack 1,  
    /// 2 -> dash release,  
    /// </param>
    public void EnableHitbox()
    {
        hitboxIsActive = true;
        print("sword hitbox activated");
        //_trailRenderer.emitting = true;
    }

    /// <summary>
    /// callback d'animation
    /// </summary>
    public void DisableHitBox()
    {
        hitboxIsActive = false;
        print("sword hitbox deactivated");
        //_trailRenderer.emitting = false;
    }

    
}
