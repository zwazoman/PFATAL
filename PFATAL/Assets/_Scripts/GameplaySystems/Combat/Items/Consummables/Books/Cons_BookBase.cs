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
    private Vector3 _vfxBaseScale;

    protected virtual void Awake()
    {
        _vfxBaseScale = _Vfx.localScale;
    }
    public override void Equip()
    {
        base.Equip();
        _spellAnimationIsPlaying = false;
        hand.animatorEventListener.OnSpellCast += OnSpellCast;
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;
        
        //reset anim
        _animator.SetBool(Active_AnimProperty,false);
        _Vfx.localScale = _vfxBaseScale;
    }

    public override void UnEquip()
    {
        //reset anim
        _animator.SetBool(Active_AnimProperty,false);
        _Vfx.localScale = _vfxBaseScale;
        
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
