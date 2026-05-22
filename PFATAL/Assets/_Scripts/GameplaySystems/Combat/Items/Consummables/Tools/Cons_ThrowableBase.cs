using System;
using DG.Tweening;
using GameplaySystems.PlayerCharacter;
using UnityEngine;

/// <summary>
/// classe de base qui gere l'activation des consommable lançables selon l'animation.
/// Les classes enfant doivent appeler "StartThrowAnimation" et override "ThrowObject".
/// </summary>
public abstract class Cons_ThrowableBase : Consummable
{
    protected bool _throwAnimationIsPlaying { get; private set; } = false;
    
    public override void Equip()
    {
        base.Equip();
        _throwAnimationIsPlaying = false;
        
        //link events
        hand.animatorEventListener.OnObjectThrown += OnObjectThrown;
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;
    }

    public override void UnEquip()
    {
        //unlink events
        hand.animatorEventListener.OnObjectThrown -= OnObjectThrown;
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;
        
        base.UnEquip();
    }

    protected void StartThrowAnimation()
    {
        if (_throwAnimationIsPlaying) return;
        
        _throwAnimationIsPlaying = true;
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.bomb_use);
    }
    
    /// <summary>
    /// appelé par l'animation, à ovveride.
    /// </summary>
    protected abstract void ThrowObject();
    
    //animation callbacks
    private void OnAnimationFinished()
    {
        if (_throwAnimationIsPlaying)
        {
            _throwAnimationIsPlaying = false;
            BreakItem();
        }
    }
    void OnObjectThrown()
    {
        ThrowObject();
    }
}
