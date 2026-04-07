using UnityEngine;

public class Cons_GroundSlam : Consummable
{
    [SerializeField] private float _downForce = 30f;

    public override void StartUsing()
    {
        base.StartUsing();

        if (playerCharacter.TryGetComponent(out PlayerPhysics physics))
        {
            physics.AddImpulse(Vector3.down * _downForce);
        }

        playerCharacter.stateMachine.s_GroundSlam.ActivateState();

        BreakItem();
    }
}