using _Scripts.StateMachine;
using System;
using UnityEngine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    /// <summary>
    /// ActivateState est appel� quand le joueur est touch� par Proj_Tornado
    /// </summary>
    [Serializable]
    public class Pst_PropulseInAir : Pst_Airborne
    {
        [SerializeField] float _damage = 3f;

        private float _startVelocityY;
        ulong _spawnerId;
        Vector3 _spawnPosition;

        public void ActivateState(ulong spawnerId, Vector3 spawnPosition)
        {
            Sm.TransitionTo(this);
            _spawnerId = spawnerId;
            _spawnPosition = spawnPosition;
        }

        protected override void OnEntered(PlayerCharacter ctx)
        {
            base.OnEntered(ctx);
            _startVelocityY = ctx.physics.Velocity.y;
        }

        public override void Behave(PlayerCharacter ctx, UpdatePoint updatePoint)
        {
            base.Behave(ctx, updatePoint);
            ApplyAirControls(ctx);
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {
            base.OnExited(playerCharacter);
            
            if (playerCharacter.TryGetComponent(out DamageableObject damageable) && playerCharacter.physics.Velocity.y >=1 )
            {
                DamageData damageData = new DamageData
                {
                    Amount = _damage * (playerCharacter.physics.Velocity.y/_startVelocityY),
                    SourcePlayerClientID = _spawnerId,
                    Point = transform.position,
                    SourcePos = _spawnPosition,
                    Direction = Vector3.down,
                };

                damageable.TakeDamage(damageData);
            }
            
        }

        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter ctx)
        {
            var nextState = base.FindNextState(ctx);
            if(nextState != this) return nextState;
            
            if (IsOnCeiling(ctx.transform.position) || ctx.physics.Velocity.y < -.1f) 
                return Sm.s_Falling;

            return this;
        }

        bool IsOnCeiling(Vector3 position)
        {
            return Physics.Raycast(position, Vector3.up, 0.6f);
        }
    }
}
