using _Scripts.StateMachine;
using System;
using UnityEngine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    [Serializable]
    public class Pst_Dead : PlayerState
    {
        bool _respawn;

        protected override void OnEntered(PlayerCharacter ctx)
        {
            base.OnEntered(ctx);

            _respawn = false;

            ctx.SwapActionMapToUI();
            ctx.HidePlayerRpc();
            ctx.physics.SetVelocity(Vector3.zero);
            ctx.inputs.Clear();
            ctx.HUD.ShowDeathUI();
            ctx.HUD.respawnButton.onClick.AddListener(Respawn);
        }

        protected override void OnExited(PlayerCharacter ctx)
        {
            base.OnExited(ctx);

            if (_respawn)
            {
                ctx.SwapActionMapToPlayer();
                ctx.ShowPlayerRpc();
                ctx.HUD.HideDeathUI();
                ctx.health.Heal();
                PlayerCharacterSpawner.Instance.SpawnPlayer(ctx);
            }

            ctx.HUD.respawnButton.onClick.RemoveListener(Respawn);
        }

        void Respawn()
        {
            _respawn = true;
        }

        public override StateBase<PlayerCharacter> FindNextState(global::_scripts.PlayerCharacter.PlayerCharacter ctx)
        {
            if(GameManager.Instance.IsGameOver)
                return Sm.s_GameOver;
            else if (_respawn)
                return Sm.s_Idle;

            return base.FindNextState(ctx);
        }
    }
}