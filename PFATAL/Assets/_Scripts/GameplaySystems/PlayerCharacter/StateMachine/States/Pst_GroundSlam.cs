using _Scripts.StateMachine;
using System;
using UnityEngine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    /// <summary>
    /// ActivateState est appelé quand le joueur utilise Cons_GroundSlam
    /// </summary>
    [Serializable]
    public class Pst_GroundSlam : Pst_Alive
    {
        private static Collider[] buffer = new Collider[20];

        [SerializeField] float _radius = 1f;
        [SerializeField] float _damage = 1f;
        [SerializeField] float _knockback = 1f;

        public void ActivateState()
        {
            Sm.TransitionTo(this);
        }

        protected override void OnEntered(PlayerCharacter playerCharacter)
        {
            playerCharacter.movement.enabled = false;
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {
            playerCharacter.movement.enabled = true;

            _radius *= playerCharacter.physics.Velocity.magnitude;
            _damage *= playerCharacter.physics.Velocity.magnitude;

            int count = Physics.OverlapSphereNonAlloc(transform.position, _radius, buffer);

            for (int i = 0; i < count; i++)
            {
                if (buffer[i].TryGetComponent(out DamageableObject hit))
                {
                    DamageData damage = new DamageData
                    {
                        Amount = _damage,
                        SourcePlayerClientID = playerCharacter.OwnerClientId,
                        Point = hit.transform.position,
                        Direction = Vector3.up,
                        KnockbackForce = new Vector3(0, _knockback, 0),
                        Radius = _radius
                    };

                    hit.TakeDamage(damage);
                }
            }
        }

        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter playerCharacter)
        {
            if (!playerCharacter.physics.ComputeIsGrounded()) return this;

            return Sm.s_Idle;
        }
    }
}
