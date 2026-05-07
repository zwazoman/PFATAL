using UnityEngine;

public class Cons_GroundSlam : Consummable
{
    [SerializeField] private float _downForce = 30f;

    [Header("Preslam Settings")]
    [SerializeField] float _upForce = 10f;
    [SerializeField] float _duration = .3f;

    public override async void StartUsing()
    {
        base.StartUsing();

        PlayerPhysics physics = playerCharacter.physics;

        physics.SetVelocity(Vector3.zero);
        physics.AddImpulse(Vector3.up * _upForce);

        await Awaitable.WaitForSecondsAsync(_duration);

        physics.SetVelocity(Vector3.zero);
        physics.AddImpulse(Vector3.down * _downForce);

        playerCharacter.stateMachine.s_GroundSlam.ActivateState();

        BreakItem();
    }
}