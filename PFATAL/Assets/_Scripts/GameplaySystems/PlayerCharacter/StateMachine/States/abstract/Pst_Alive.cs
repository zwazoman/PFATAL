using _Scripts.StateMachine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    public abstract class Pst_Alive : PlayerState
    {
        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter ctx)
        {
            if (ctx.health.IsDead)
                return Sm.s_dead;

            return base.FindNextState(ctx);
        }
    }
}