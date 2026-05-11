using System;
using GameplaySystems.PlayerCharacter;
using UnityEngine;

public abstract class Cons_RuneBase : Consummable
{
    protected bool _breakAnimationIsPlaying { get; private set; } = false;

    public override void Equip()
    {
        base.Equip();
        _breakAnimationIsPlaying = false;
        hand.animatorEventListener.OnGemBroken += OnGemBroken;
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;
    }

    public override void UnEquip()
    {
        base.UnEquip();
        hand.animatorEventListener.OnGemBroken -= OnGemBroken;
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;
    }

    protected void StartBreakingAnimation()
    {
        if (_breakAnimationIsPlaying) return;
        _breakAnimationIsPlaying = true;
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.gem_break);
    }
    
    /// <summary>
    /// appelé par l'animation, à ovveride.
    /// </summary>
    protected abstract void ApplyGemEffect();
    
    //animation callbacks
    private void OnAnimationFinished()
    {
        if (_breakAnimationIsPlaying)
        {
            _breakAnimationIsPlaying = false;
            BreakItem();
        }
    }
    void OnGemBroken()
    {
        ApplyGemEffect();
    }
    
}
