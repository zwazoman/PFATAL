using System;
using _Scripts.StateMachine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    /// <summary>
    /// on entre dans cet état à la fin de la partie, après GameManager.EventOnGameEnded.
    /// Le personnage ne peut plus rien faire.
    /// </summary>
    [Serializable]
    public class Pst_GameOver : PlayerState
    {
        protected override void OnEntered(PlayerCharacter ctx)
        {
            base.OnEntered(ctx);
            ctx.health.enabled = false;
            ctx.physics.enabled = false;
            ctx.inputs.enabled = false;
        }

        public override StateBase<global::_scripts.PlayerCharacter.PlayerCharacter> FindNextState(global::_scripts.PlayerCharacter.PlayerCharacter ctx)
        {
            return base.FindNextState(ctx); 
        }
    }
}