using _Scripts.StateMachine;
using Unity.VisualScripting;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    public abstract class Pst_Alive : PlayerState
    {
        bool _died;

        protected override void OnEntered(PlayerCharacter ctx)
        {
            base.OnEntered(ctx);

            ctx.health.OnDie += Die;
        }

        protected override void OnExited(PlayerCharacter ctx)
        {
            base.OnExited(ctx);

            ctx.health.OnDie -= Die;
        }

        void Die()
        {
            _died = true;
        }

        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter ctx)
        {
            if(GameManager.Instance.IsGameOver)
                return Sm.s_GameOver;
            
            else if (_died)
            {
                _died = false;
                return Sm.s_dead;
            }
                
            return base.FindNextState(ctx);
        }
    }
}