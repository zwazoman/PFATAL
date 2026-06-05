using System;
using _Scripts.Pooling;
using GameplaySystems.PlayerCharacter;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public abstract class Cons_RuneBase : Consummable
{
    protected bool _breakAnimationIsPlaying { get; private set; } = false;

    [SerializeField] MeshRenderer _gemstoneRenderer;
    [SerializeField] [ColorUsage(true,true)]private Color _vfxColor;

    public override void Equip()
    {
        base.Equip();
        
        //reset variables
        _breakAnimationIsPlaying = false;
        _gemstoneRenderer.enabled = true;
        
        //link events
        hand.animatorEventListener.OnGemBroken += OnGemBroken;
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;
    }

    public override void UnEquip()
    {
        base.UnEquip();
        
        //unlink events
        hand.animatorEventListener.OnGemBroken -= OnGemBroken;
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;
    }

    protected void StartBreakingAnimation()
    {
        if (_breakAnimationIsPlaying) return;
        
        StopScrollable();
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
            Scrollable();
            BreakItem();
        }
    }
    void OnGemBroken()
    {
        //feedbacks
        PooledObject po = LocalPoolManager.Instance.Pool_VFX_GemBreak.PullObjectFromPool();
        po.transform.position = _gemstoneRenderer.transform.position;
        po.transform.parent = hand.transform;
        po.gameObject.layer = hand.playerCharacter.visuals.IsInFpsView
            ? PlayerCharacterVisuals.fpsLayerMask
            : PlayerCharacterVisuals.defaultLayerMask;
        po.GetComponent<VisualEffect>().SetVector4("Color",_vfxColor);
        po.GoBackIntoPool_Delayed(1f);
        _gemstoneRenderer.enabled = false;

        print("Gemstone Destroyed");
        
        //gameplay effect
        ApplyGemEffect();
    }
    
}
