using _Scripts.StateMachine;
using System;
using UnityEngine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    /// <summary>
    /// ActivateState est appelé quand le joueur est touché par Proj_Tornado
    /// </summary>
    [Serializable]
    public class Pst_PropulseInAir : Pst_Alive
    {
        [SerializeField] float _damage = 3f;

        ulong _ownerId;

        public void ActivateState(ulong ownerId)
        {
            Sm.TransitionTo(this);
            _ownerId = ownerId;
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {
            if (playerCharacter.TryGetComponent(out DamageableObject damageable) && playerCharacter.physics.Velocity.y >= 1)
            {
                DamageData damage = new DamageData
                {
                    Amount = _damage,
                    SourcePlayerClientID = _ownerId,
                    Point = transform.position,
                    Direction = Vector3.down,
                    KnockbackForce = Vector3.zero,
                    Radius = 0
                };

                damageable.TakeDamage(damage);
            }
        }

        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter playerCharacter)
        {
            if (!IsOnCeiling(playerCharacter.transform.position) && playerCharacter.physics.Velocity.y >= -1) return this;

            return Sm.s_Falling;
        }

        bool IsOnCeiling(Vector3 position)
        {
            return Physics.Raycast(position, Vector3.up, 0.6f);
        }
    }
}
