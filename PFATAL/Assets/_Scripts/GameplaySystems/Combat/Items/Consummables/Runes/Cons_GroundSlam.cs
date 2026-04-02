using UnityEngine;

public class Cons_GroundSlam : Consummable
{
    [SerializeField] private float _downForce = 30f;

    public override void StartUsing()
    {
        base.StartUsing();

        // boost vers le bas
        if (playerCharacter.TryGetComponent(out PlayerPhysics physics))
        {
            physics.AddImpulse(Vector3.down * _downForce);
        }

        // activer state

        BreakItem();
    }
}