using System;
using _scripts.PlayerCharacter;
using _Scripts.StateMachine;
using UnityEngine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    /// <summary>
    /// Freeze est appelé quand le joueur utilise Con_BigLaserBeam pour l'instant
    /// </summary>
    [Serializable]
    public class Pst_Frozen : Pst_Alive
    {
        public float duration = 2f;
        
        private float _endTime;

        public void Freeze(float seconds)
        {
            duration = seconds;
            Sm.TransitionTo(this);
        }

        protected override void OnEntered(PlayerCharacter playerCharacter)
        {
            _endTime = Time.time + duration;
            
            playerCharacter.physics.SetVelocity(Vector3.zero);
            playerCharacter.physics.enabled = false;
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {
            playerCharacter.physics.enabled = true;
        }

        public override void Behave(PlayerCharacter playerCharacter, UpdatePoint updatePoint)
        {
            if (updatePoint != UpdatePoint.Update) return;
            
            playerCharacter.physics.SetVelocity(Vector3.zero);
        }

        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter playerCharacter)
        {
            if (Time.time >= _endTime)
                return playerCharacter.physics.ComputeIsGrounded() ? Sm.s_Idle : Sm.s_Falling;

            return base.FindNextState(playerCharacter);
        }
    }
}