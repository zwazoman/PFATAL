using System;
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
        public float duration = 1f;
        
        private float _endTime;

        public void Freeze(float seconds)
        { 
            Print("[BigLaserBeam] Freeze for " + seconds + " seconds");
            duration = seconds;
            Sm.TransitionTo(this);
           
        }
        
        public void Unfreeze()
        {
            Print("[BigLaserBeam] Unfreeze");
            _endTime = Time.time;
        }

        protected override void OnEntered(PlayerCharacter playerCharacter)
        {
            base.OnEntered(playerCharacter);
            _endTime = Time.time + duration;
            playerCharacter.physics.SetVelocity(Vector3.zero);
            playerCharacter.physics.enabled = false;
            Print("[BigLaserBeam] Entered Frozen state, will end at " + _endTime);
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {
            base.OnExited(playerCharacter);
            playerCharacter.physics.enabled = true;
            Print("[BigLaserBeam] Exited Frozen state");
        }

        public override void Behave(PlayerCharacter playerCharacter, UpdatePoint updatePoint)
        {
            base.Behave(playerCharacter, updatePoint);
            playerCharacter.physics.SetVelocity(Vector3.zero);
        }

        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter playerCharacter)
        {
            var nextState = base.FindNextState(playerCharacter);
            if(nextState != this) return nextState;
            
            if (Time.time >= _endTime)
                return playerCharacter.physics.ComputeIsGrounded() ? Sm.s_Idle : Sm.s_Falling;

            return this;
        }
    }
}