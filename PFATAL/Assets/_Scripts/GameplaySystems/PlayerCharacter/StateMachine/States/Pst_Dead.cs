using _Scripts.StateMachine;
using System.Collections.Generic;
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
            ctx.physics.SetVelocity(Vector3.zero);
            ctx.inputs.Clear();
            ctx.HUD.ShowDeathUI();
            ctx.visuals.HideRpc();
            
            //nettoie les mains et drop le consommable actuel
            ctx.playerHands.leftHand.DropEquippedtem();
            ctx.playerHands.ClearHands();

            ctx.inputs.OnRespawnInput += Respawn;
        }

        protected override void OnExited(PlayerCharacter ctx)
        {
            base.OnExited(ctx);

            if (_respawn)
            {
                _respawn = false;
                ctx.SwapActionMapToPlayer();
                ctx.HUD.HideDeathUI();
                ctx.health.Heal();
                ctx.playerHands.TryEquipRandomWeapon();
                ctx.visuals.ShowRpc();
                PlayerCharacterSpawner.Instance.ReSpawnPlayer(ctx);
            }

            ctx.inputs.OnRespawnInput -= Respawn;
        }

        public override void Behave(PlayerCharacter ctx, UpdatePoint updatePoint)
        {
            base.Behave(ctx, updatePoint);
            ctx.physics.SetVelocity(Vector3.zero);
        }

        void Respawn()
        {
            _respawn = true;
        }

        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter ctx)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                Debug.Log("returned game over.");
                return Sm.s_GameOver;
            }
            else if (_respawn)
            {
                Debug.Log("returned idle.");
                return Sm.s_Idle;
            }

            return base.FindNextState(ctx);
        }
    }
}