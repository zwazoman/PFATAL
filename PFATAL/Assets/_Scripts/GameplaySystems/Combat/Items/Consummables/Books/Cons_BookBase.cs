using System;
using DG.Tweening;
using GameplaySystems.PlayerCharacter;
using UnityEngine;

public abstract class Cons_BookBase : Consummable
{
    private static readonly int Active_AnimProperty = Animator.StringToHash("Active");
    
    protected bool _spellAnimationIsPlaying { get; private set; } = false;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _Vfx;
    
    public override void Equip()
    {
        base.Equip();
        _spellAnimationIsPlaying = false;
        hand.animatorEventListener.OnSpellCast += OnSpellCast;
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;
    }

    public override void UnEquip()
    {
        //reset anim
        _animator.SetBool(Active_AnimProperty,false);
        _Vfx.transform.position = Vector3.one;
        
        //unlink events
        hand.animatorEventListener.OnSpellCast -= OnSpellCast;
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;
        
        base.UnEquip();
    }

    protected void StartCastAnimation()
    {
        if (_spellAnimationIsPlaying) return;
        
        _spellAnimationIsPlaying = true;
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.book_use);
        _animator.SetBool(Active_AnimProperty,true);
        _Vfx.DOScale(0,.5f).SetEase(Ease.OutQuad);
        
    }
    
    /// <summary>
    /// appelé par l'animation, à ovveride.
    /// </summary>
    protected abstract void ApplySpellEffect();
    
    //animation callbacks
    private void OnAnimationFinished()
    {
        if (_spellAnimationIsPlaying)
        {
            _spellAnimationIsPlaying = false;
            BreakItem();
        }
    }
    void OnSpellCast()
    {
        ApplySpellEffect();
    }
    
}
