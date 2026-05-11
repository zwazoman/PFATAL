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
        [SerializeField] private float _upForce = 10f;
        [SerializeField] private float _preSlamDuration = 0.3f;
        [SerializeField] private float _slamGravitiMultiplier = 5f;
        [SerializeField] float _radius = 1f;
        [SerializeField] int _baseDamage = 2;
        [SerializeField] int _maxDamage = 7;
        [SerializeField] float _damageMultiplier = 1.5f;
        [SerializeField] float _knockback = 1f;
        [SerializeField] LayerMask _playerLayer;

        private float _startY;
        private float gravityScaleBeforeSlam;

        public void ActivateState()
        {
            Sm.TransitionTo(this);
        }

        protected override async void OnEntered(PlayerCharacter playerCharacter)
        {
            base.OnEntered(playerCharacter);

            gravityScaleBeforeSlam = playerCharacter.physics.GetGravityStrength();

            playerCharacter.movement.enabled = false;

            // Phase pre-slam : petite impulsion vers le haut
            playerCharacter.physics.SetVelocity(Vector3.zero);
            playerCharacter.physics.AddImpulse(Vector3.up * _upForce);

            await Awaitable.WaitForSecondsAsync(_preSlamDuration);

            // Transition vers la phase slam : annule la vélocité et change la gravité
            playerCharacter.physics.SetVelocity(Vector3.zero);
            playerCharacter.physics.ChangeGravityStrenght(gravityScaleBeforeSlam * _slamGravitiMultiplier);

            _startY = playerCharacter.transform.position.y;
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {
            base.OnExited(playerCharacter);

            // Restaure la gravité normale
            playerCharacter.physics.ChangeGravityStrenght(gravityScaleBeforeSlam);
            playerCharacter.movement.enabled = true;

            float fallHeight = _startY - playerCharacter.transform.position.y;
            float rawDamage = _baseDamage + (fallHeight * _damageMultiplier);
            int finalDamage = Mathf.Clamp(Mathf.RoundToInt(rawDamage), _baseDamage, _maxDamage);
            float radius = _radius * (1 + fallHeight * 0.1f);

            int count = Physics.OverlapSphereNonAlloc(
                playerCharacter.transform.position, radius, buffer, _playerLayer);

            for (int i = 0; i < count; i++)
            {
                if (buffer[i].TryGetComponent(out DamageableObject hit))
                {
                    if (hit.gameObject == playerCharacter.gameObject) continue; // fix: return -> continue

                    DamageData damageData = new DamageData
                    {
                        Amount = finalDamage,
                        SourcePlayerClientID = playerCharacter.OwnerClientId,
                        Point = hit.transform.position,
                        SourcePos = playerCharacter.transform.position,
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
            var nextState = base.FindNextState(playerCharacter);
            if (nextState != this) return nextState;

            if (playerCharacter.physics.ComputeIsGrounded())
                return Sm.s_Idle;

            return this;
        }
    }
}
