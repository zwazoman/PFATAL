using GameplaySystems.PlayerCharacter;
using UnityEngine;

public class Cons_Heal : Consummable
{
    [SerializeField] private float _heal = 5f;

    private bool _isPlayingBreakAnimation = false;
    public override void Equip()
    {
        base.Equip();
        _isPlayingBreakAnimation = false;
        
        //link events
        hand.animatorEventListener.OnGemBroken += BreakGem;
        hand.animatorEventListener.OnAnimationFinished+= OnAnimationFinished;
    }

    public override void UnEquip()
    {
        base.UnEquip();
        //unlink events
        hand.animatorEventListener.OnGemBroken -= BreakGem;
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;
    }

    //appelé par un event de l'animation
    void OnAnimationFinished()
    {
        if (_isPlayingBreakAnimation)
        {
            _isPlayingBreakAnimation = false;
            BreakItem();
        }
    }

    //appelé par un event de l'animation
    void BreakGem()
    {
        if (playerCharacter.TryGetComponent(out DamageableObject health))
        {
            health.Heal(_heal);
        }
    }
    
    //quand on click
    public override void StartUsing()
    {
        base.StartUsing();

        //lance l'anim de break
        if(_isPlayingBreakAnimation) return; 
        _isPlayingBreakAnimation = true;
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.gem_break);
    }
}