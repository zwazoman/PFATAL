using GameplaySystems.PlayerCharacter;
using UnityEngine;

public class Cons_GroundSlam : Consummable
{
    [SerializeField] private float _downForce = 30f;

    [Header("Preslam Settings")]
    [SerializeField] float _upForce = 10f;
    [SerializeField] float _duration = .3f;

    bool breakAnimIsPlaying = false;

    public override void Equip()
    {
        base.Equip();
     
        //link animation events
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;
        hand.animatorEventListener.OnGemBroken += OnGemBroken;
    }

    public override void UnEquip()
    {
        base.UnEquip();
        
        //unlink animation events
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;
        hand.animatorEventListener.OnGemBroken -= OnGemBroken;
    }

    public override async void StopUsing()
    {
        base.StopUsing();

        if (!breakAnimIsPlaying)
        {
            breakAnimIsPlaying = true;
            hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.gem_break);
        }
    }

    //appelé par un event de l'animation
    void OnGemBroken()
    {
        PlayerPhysics physics = playerCharacter.physics;

        //todo : "faites mieux"
        physics.SetVelocity(Vector3.zero);
        physics.AddImpulse(Vector3.up * _upForce);
        //await Awaitable.WaitForSecondsAsync(_duration);
        physics.SetVelocity(Vector3.zero);
        physics.AddImpulse(Vector3.down * _downForce);

        playerCharacter.stateMachine.s_GroundSlam.ActivateState();
        
    }

    //appelé par un event de l'animation
    void OnAnimationFinished()
    {
        //retirer l'item à la fin de l'anim de break
        if (breakAnimIsPlaying)
        {
            breakAnimIsPlaying = false;
            BreakItem();
        }
    }
}