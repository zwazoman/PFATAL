using _Scripts.StateMachine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    public class Pst_Dead : PlayerState
    {
        protected override void OnEntered(PlayerCharacter ctx)
        {
            base.OnEntered(ctx);

            ctx.SwapActionMapToUI();
            ctx.HidePlayer();
        }

        protected override void OnExited(PlayerCharacter ctx)
        {
            base.OnExited(ctx);
        }

        public override void Behave(PlayerCharacter ctx, UpdatePoint updatePoint)
        {
            base.Behave(ctx, updatePoint);
        }

        public override StateBase<PlayerCharacter> FindNextState(global::_scripts.PlayerCharacter.PlayerCharacter ctx)
        {
            return this;
        }
    }
}