using _Scripts.StateMachine;
using System;
using UnityEngine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    /// <summary>
    /// ActivateState est appelé quand le joueur utilise Cons_GroundSlam
    /// </summary>
    [Serializable]
    public class Pst_GroundSlam : Pst_Alive
    {
        private static Collider[] buffer = new Collider[20];

        [Header("Ground Slam Settings")]
        [SerializeField] float _radius = 1f;
        [SerializeField] int _baseDamage = 2;
        [SerializeField] int _maxDamage = 7;
        [SerializeField] float _damageMultiplier = 0.5f;
        [SerializeField] float _knockback = 1f;

        private float _startY;

        public void ActivateState()
        {
            Sm.TransitionTo(this);
        }

        protected override void OnEntered(PlayerCharacter playerCharacter)
        {
            playerCharacter.movement.enabled = false;

            _startY = playerCharacter.transform.position.y;
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {
            playerCharacter.movement.enabled = true;

            float velocity = Mathf.Abs(playerCharacter.physics.Velocity.y);
            float fallHeight = _startY - playerCharacter.transform.position.y;

            float rawDamage = _baseDamage + (fallHeight * _damageMultiplier);
            rawDamage = Mathf.Clamp(rawDamage, _baseDamage, _maxDamage);
            int finalDamage = Mathf.Clamp(Mathf.RoundToInt(rawDamage), (int)_baseDamage, (int)_maxDamage);
            float radius = _radius * (1 + fallHeight * 0.1f);


            int count = Physics.OverlapSphereNonAlloc(playerCharacter.transform.position, _radius, buffer);

            for (int i = 0; i < count; i++)
            {
                if (buffer[i].TryGetComponent(out DamageableObject hit))
                {
                    if (hit.gameObject == playerCharacter.gameObject) return;

                    DamageData damageData = new DamageData
                    {
                        Amount = finalDamage,
                        SourcePlayerClientID = playerCharacter.OwnerClientId,
                        Point = hit.transform.position,
                        Direction = Vector3.up,
                        KnockbackForce = new Vector3(0, _knockback, 0),
                        Radius = radius
                    };

                    hit.TakeDamage(damageData);

                    Debug.Log($"Ground Slam hit {hit.name} for {damageData.Amount}");
                }
            }
        }

        public override StateBase<PlayerCharacter> FindNextState(PlayerCharacter playerCharacter)
        {
            if (!playerCharacter.physics.ComputeIsGrounded()) return this;

            return Sm.s_Idle;
        }
    }
}
