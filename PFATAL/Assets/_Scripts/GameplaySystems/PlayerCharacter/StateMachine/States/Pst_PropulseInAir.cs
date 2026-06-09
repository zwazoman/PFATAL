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

            if (IsOnCeiling(ctx.transform.position) && ctx.TryGetComponent(out DamageableObject damageable) && ctx.physics.Velocity.y >= 1)
            {
                DamageData damageData = new DamageData
                {
                    Amount = _damage,
                    SourcePlayerClientID = _spawnerId,
                    Point = transform.position,
                    SourcePos = _spawnPosition,
                    Direction = Vector3.down,
                };

                Print($"[Pst_PropulseInAir] {damageData.Amount}");
                damageable.TakeDamage(damageData);
            }
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {   
            base.OnExited(playerCharacter);
        }

        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter ctx)
        {
            var nextState = base.FindNextState(ctx);
            if(nextState != this) return nextState;
            
            if (ctx.physics.Velocity.y < -.1f) 
                return Sm.s_Falling;

            return this;
        }

        bool IsOnCeiling(Vector3 position)
        {
            return Physics.Raycast(position, Vector3.up, 0.6f);
        }
    }
}
