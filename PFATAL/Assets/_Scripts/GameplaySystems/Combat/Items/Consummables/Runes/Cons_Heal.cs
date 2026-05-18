using GameplaySystems.PlayerCharacter;
using UnityEngine;

public class Cons_Heal : Cons_RuneBase
{
    [SerializeField] private float _heal = 5f;
    [SerializeField] float _moveSpeedBoostMultiplyer = 10f;
    [SerializeField] float _moveSpeedBoostDuration = 5;

    //appelé par un event de l'animation
    protected override void ApplyGemEffect()
    {
        playerCharacter.health.Heal(_heal);
        playerCharacter.movement.TemporaryMoveSpeedChange(_moveSpeedBoostMultiplyer, _moveSpeedBoostDuration);

        hand.playerCharacter.visuals.PlayHealingVFX();
    }

    //quand on click
    public override void StartUsing()
    {
        base.StartUsing();

        //lance l'anim de break
        StartBreakingAnimation();
    }

}