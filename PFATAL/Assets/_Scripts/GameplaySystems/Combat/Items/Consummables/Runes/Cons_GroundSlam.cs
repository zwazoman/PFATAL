using GameplaySystems.PlayerCharacter;
using UnityEngine;

public class Cons_GroundSlam : Cons_RuneBase
{
    [SerializeField] private float _downForce = 30f;

    [Header("Preslam Settings")]
    [SerializeField] float _upForce = 10f;
    [SerializeField] float _duration = .3f;

    public override void StopUsing()
    {
        base.StopUsing();

        if (!_breakAnimationIsPlaying)
        {
            StartBreakingAnimation();
        }
    }

    //appelé par un event de l'animation
    protected override void ApplyGemEffect()
    {
        playerCharacter.stateMachine.s_GroundSlam.ActivateState();   
    }

}