using _Scripts.StateMachine;
using System;
using _Scripts.Pooling;
using DG.Tweening;
using SimpleVFXs;
using Unity.VisualScripting;
using UnityEngine;

namespace _scripts.PlayerCharacter.StateMachine.States
{
    /// <summary>
    /// ActivateState est appel� quand le joueur utilise Cons_GroundSlam
    /// </summary>
    [Serializable]
    public class Pst_GroundSlam : Pst_Airborne
    {
        private static Collider[] buffer = new Collider[20];
        
        [Header("Ground Slam movement Settings")]
        [SerializeField] private float _preSlamDuration = 0.3f;
        [SerializeField] private float _upForce = 2.5f;
        [SerializeField] private float _velocityMultiplier = 2.5f;
        [SerializeField] private float _slamGravitiMultiplier = 5f;
        [SerializeField] float FovOffsetStrength = 10;
        
        [Header("Ground Slam impact Settings")]
        [SerializeField] float _radius = 4f;
        [SerializeField] int _baseDamage = 2;
        [SerializeField] int _maxDamage = 7;
        [SerializeField] float _damageMultiplier = 1.5f;
        [SerializeField] float _knockback = 1f;
        [SerializeField] LayerMask _playerLayer;

        private float _startY;
        private float gravityScaleBeforeSlam;
        private bool slamPhaseStarted = false;

        private float _fovOffset;
        
        
        public void ActivateState()
        {
            Sm.TransitionTo(this);
        }

        public override void Behave(PlayerCharacter ctx, UpdatePoint updatePoint)
        {
            if (updatePoint == UpdatePoint.FixedUpdate)
            {
                //déplacements
                ApplyAirControls(ctx);
            }
            else if (updatePoint == UpdatePoint.Update)
            {
                //gestion de la FOV dans update
                ctx.cameraBehaviour.AddTemporaryFovOffset(_fovOffset);
            }
            
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

            //animation de la fov de la camera
             DOTween.To(
                 () => _fovOffset,
                 v =>
                 {
                     _fovOffset = v;
                     //PlayerCharacter.LocalPlayerCharacter.cameraBehaviour.AddTemporaryFovOffset(v);
                 },
                 FovOffsetStrength, .2f
             ).SetEase(Ease.InOutSine);
             
            // Phase pre-slam : petite impulsion vers le haut
            Vector3 playerVelocity = playerCharacter.physics.Velocity;
            playerCharacter.physics.SetVelocity(new Vector3(playerVelocity.x * _velocityMultiplier, _upForce, playerVelocity.z * _velocityMultiplier)+playerCharacter.transform.forward * (_upForce * .35f));

            await Awaitable.WaitForSecondsAsync(_preSlamDuration);

            // Transition vers la phase slam : annule la v�locit� et change la gravit�
            playerCharacter.physics.SetVelocity(new Vector3(playerCharacter.physics.Velocity.x*.6f, playerCharacter.physics.Velocity.x*.3f, playerCharacter.physics.Velocity.z)*.6f);
            playerCharacter.physics.SetGravityStrength(gravityScaleBeforeSlam * _slamGravitiMultiplier);

            _startY = playerCharacter.transform.position.y;
            slamPhaseStarted = true;
        }

        protected override void OnExited(PlayerCharacter playerCharacter)
        {
            base.OnExited(playerCharacter);
            
            Print("slam exited");
            
            slamPhaseStarted = false;

            //animation de la fov de la camera
            DOTween.To(
                () => _fovOffset,
                v =>
                {
                    _fovOffset = v;
                    PlayerCharacter.LocalPlayerCharacter.cameraBehaviour.AddTemporaryFovOffset(v);
                },
                0, .2f
            ).SetEase(Ease.InOutSine);
            
            //vfx
            PooledObject vfx = LocalPoolManager.Instance.Pool_VFX_GroundSlam.PullObjectFromPool(transform.position+Vector3.down*.5f);
            vfx.GetComponent<StylisedEffect>().TriggerMainEvent();
            vfx.GoBackIntoPool_Delayed(3);
            
            // Restaure la gravit� normale
            playerCharacter.physics.SetGravityStrength(gravityScaleBeforeSlam);
            playerCharacter.movement.enabled = true;
            
            //applique les degats aux joueurs autour
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

            if (slamPhaseStarted && playerCharacter.physics.Velocity.y<0 && (playerCharacter.physics.ComputeIsGrounded() || playerCharacter.physics.ComputeIsBumpered()))
                return Sm.s_Idle;

            return this;
        }
    }
}
