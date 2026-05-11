using GameplaySystems.PlayerCharacter;
using UnityEngine;

public class Cons_Heal : Cons_RuneBase
{
    [SerializeField] private float _heal = 5f;
    
    //appelé par un event de l'animation
    protected override void ApplyGemEffect()
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
        StartBreakingAnimation();
    }
    
}