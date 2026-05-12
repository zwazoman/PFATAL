using _Scripts.StateMachine;
using System;
using UnityEngine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    /// <summary>
    /// ActivateState est appelé quand le joueur utilise Cons_GroundSlam
    /// </summary>
    [Serializable]
    public class Pst_GroundSlam : Pst_Airborne
    {
        private static Collider[] buffer = new Collider[20];


        [Header("Ground Slam Settings")]
        [SerializeField] private float _upForce = 2.5f;
        [SerializeField] private float _preSlamDuration = 0.3f;
        [SerializeField] private float _velocityMultiplier = 2.5f;
        [SerializeField] private float _slamGravitiMultiplier = 5f;
        [SerializeField] float _radius = 1f;
        [SerializeField] int _baseDamage = 2;
        [SerializeField] int _maxDamage = 7;
        [SerializeField] float _damageMultiplier = 1.5f;
        [SerializeField] float _knockback = 1f;
        [SerializeField] LayerMask _playerLayer;

        private float _startY;
        private float gravityScaleBeforeSlam;
        private bool slamPhaseStarted = false;

        public void ActivateState()
        {
            Sm.TransitionTo(this);
        }

        public override void Behave(PlayerCharacter ctx, UpdatePoint updatePoint)
        {
            ApplyAirControls(ctx);
            base.Behave(ctx, updatePoint);
        }

        public override void SetUp(StateMachine<PlayerCharacter> playerStateMachine)
        {
            base.SetUp(playerStateMachine);
            gravityScaleBeforeSlam = playerStateMachine.GetComponent<PlayerPhysics>().GetGravityStrength();
        }

        protected override async void OnEntered(PlayerCharacter playerCharacter)
        {
            base.OnEntered(playerCharacter);

            playerCharacter.movement.enabled = false;

            // Phase pre-slam : petite impulsion vers le haut
            Vector3 playerVelocity = playerCharacter.physics.Velocity;
            playerCharacter.physics.SetVelocity(new Vector3(playerVelocity.x * _velocityMultiplier, 0, playerVelocity.z * _velocityMultiplier));
            playerCharacter.physics.AddImpulse(Vector3.up * _upForce);

            await Awaitable.WaitForSecondsAsync(_preSlamDuration);

            // Transition vers la phase slam : annule la vélocité et change la gravité
            //playerCharacter.physics.SetVelocity(Vector3.zero);
            playerCharacter.physics.ChangeGravityStrenght(gravityScaleBeforeSlam * _slamGravitiMultiplier);

            _startY = playerCharacter.transform.position.y;
            slamPhaseStarted = true;
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {
            base.OnExited(playerCharacter);

            slamPhaseStarted = false;

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

            if (slamPhaseStarted && (playerCharacter.physics.ComputeIsGrounded() || playerCharacter.physics.ComputeIsBumpered()))
                return Sm.s_Idle;

            return this;
        }
    }
}
