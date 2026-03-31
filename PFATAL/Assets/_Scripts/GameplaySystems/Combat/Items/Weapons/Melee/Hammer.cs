using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Hammer : MeleeWeapon
{
    [Header("Hammer References")]
    [SerializeField] Animator _animator;
    [SerializeField] HammerEventReceiver _eventReceiver;

    [Header("Hammer Settings")]
    [SerializeField] float _dashChargedDuration;
    [SerializeField] float _dashStrength = 7;
    [SerializeField] float _dashDmgMult = .8f;

    bool _dashed;
    bool _charged;
    bool _isAttacking;

    public override void Equip()
    {
        base.Equip();

        _animator.SetTrigger("Idle");

        _charged = false;
        _dashed = false;
        _isAttacking = false;
        isHitting = false;

        _eventReceiver.OnHitStart += StartHitting;
        _eventReceiver.OnHitEnd += StopHitting;
    }

    public override void UnEquip()
    {
        base.UnEquip();

        _eventReceiver.OnHitStart -= StartHitting;
        _eventReceiver.OnHitEnd -= StopHitting;
    }

    [Rpc(SendTo.Server)]
    protected override void ApplyHitRpc(DamageableObject damageable)
    {
        base.ApplyHitRpc(damageable);

        print("hit hammer");

        DamageData data = new();
        data.Point = hitSocket.position;
        data.Direction = transform.forward;

        if(_dashed)
            data.Amount = damageAmount * _dashDmgMult;
        else
            data.Amount = damageAmount;

        data.Radius = hitSphereRadius;
        data.SourcePlayerClientID = playerCharacter.OwnerClientId;
        data.KnockbackForce = playerCharacter.transform.forward * 5;

        damageable.TakeDamage(data);

    }

    public override void UseUpdate()
    {
        base.UseUpdate();

        if(holdDuration >= _dashChargedDuration && !_charged && !_isAttacking)
        {
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
            playerCharacter.physics.AddImpulse(playerCharacter.playerCamera.transform.forward * _dashStrength);
            _charged = false;
        }
        else
            _animator.SetTrigger("Hit");

        _isAttacking = true;
    }

    /// <summary>
    /// callback d'animation
    /// </summary>
    public void StartHitting(bool dashed)
    {
        print("start");

        _dashed = dashed;
        isHitting = true;
    }

    /// <summary>
    /// callback d'animation
    /// </summary>
    public void StopHitting() 
    {
        isHitting = false; print("stop");
        _isAttacking = false;
    }
}
