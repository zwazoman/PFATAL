using System;
using GameplaySystems.PlayerCharacter;
using UnityEngine;

public abstract class BookBase : Consummable
{
    protected bool _spellAnimationIsPlaying { get; private set; } = false;

    public override void Equip()
    {
        base.Equip();
        _spellAnimationIsPlaying = false;
        hand.animatorEventListener.OnSpellCast += OnGemBroken;
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;
    }

    public override void UnEquip()
    {
        base.UnEquip();
        hand.animatorEventListener.OnSpellCast -= OnGemBroken;
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;
    }

    protected void StartBreakingAnimation()
    {
        if (_spellAnimationIsPlaying) return;
        
        _spellAnimationIsPlaying = true;
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.gem_break);
    }
    
    /// <summary>
    /// appelé par l'animation, à ovveride.
    /// </summary>
    protected abstract void ApplyGemEffect();
    
    //animation callbacks
    private void OnAnimationFinished()
    {
        if (_spellAnimationIsPlaying)
        {
            _spellAnimationIsPlaying = false;
            BreakItem();
        }
    }
    void OnGemBroken()
    {
        ApplyGemEffect();
    }
    
}
