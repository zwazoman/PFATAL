using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

namespace GameplaySystems.PlayerCharacter
{
    public class PlayerHandVisuals : NetworkBehaviour
    {
        public static float CrossbowAnimationSpeedMultiplier = 1;
        public enum AnimationID
        {
            //voir _Graph/Meshes/Chara/AC_HandCharacter.controller

            unknown = -1,
            currentDefaultIdlePose = 0,
            freeHands_idle = 1,

            crossbow_idle = 100,
            crossbow_shootAndReload = 101,

            tomahawk_idle = 200,
            tomahawk_throw = 201,
            tomahawk_grapple = 202,

            sword_idle = 300,
            sword_attack_small = 301,
            sword_charge_release = 303,
            sword_charge_idle = 304,

            book_idle = 400,
            book_use = 401,
            book_charge_idle = 402,

            gem_idle = 500,
            gem_break = 501,

            bomb_idle = 600,
            bomb_use = 601,
        }

        //animator properties ids
        private static readonly int MainAnim_AnimatorProperty = Animator.StringToHash("mainAnim");
        private static readonly int AdditiveAnim_AnimatorProperty = Animator.StringToHash("AdditiveAnim");
        private static readonly int CrossbowShootAnimationSpeedMultiplier_AnimatorProperty = Animator.StringToHash("crossbowShootAnimationSpeedMultiplier");
        private static readonly int TomahawkThrowAnimationSpeedMultiplier_AnimatorProperty = Animator.StringToHash("TomahawkThrowAnimationSpeedMultiplier");
        private static readonly int PlayMainAnimation_AnimatorProperty = Animator.StringToHash("PlayMainAnimation");
        private static readonly int PlayAdditiveAnim_AnimatorProperty = Animator.StringToHash("PlayAdditiveAnim");
        private static readonly int IsRunning_AnimatorProperty = Animator.StringToHash("isRunning");
        private static readonly int PlayPickupAnimation_AnimatorProperty = Animator.StringToHash("PlayPickupAnimation");

        [Header("Scene references")]
        [SerializeField] Hand _hand;
        [SerializeField] Animator _animator;

        private AnimationID currentDefaultIdlePose;

        NetworkBehaviour[] behaviours;

        void Awake()
        {
            _hand.OnEquipItem += OnNewItemEquipped;
            //_hand.OnSwapItem += OnSwapItem;
            _hand.OnUnequipItem += OnItemUnequipped;
            _hand.OnDeleteItem += PlayCurrentDefaultIdlePoseAnimation;
            _hand.OnDropItem += OnItemUnequipped;
            _hand.animatorEventListener.OnAnimationFinished += PlayCurrentDefaultIdlePoseAnimation;

            behaviours = _hand.playerCharacter.GetComponentsInChildren<NetworkBehaviour>();
        }

        private void Update()
        {
            _animator.SetBool(IsRunning_AnimatorProperty, _hand.playerCharacter.physics.Velocity.sqrMagnitude > .25f);
        }

        private void OnSwapItem()
        {
            print("received event swap item");
            OnItemUnequipped(_hand.equippedItem);
        }

        private void OnNewItemEquipped(Item equippedItem)
        {
            //set idle pose
            currentDefaultIdlePose = equippedItem switch
            {
                Crossbow => AnimationID.crossbow_idle,
                Tomahawk => AnimationID.tomahawk_idle,
                Sword => AnimationID.sword_idle,

                Cons_Tornado => AnimationID.book_idle,
                Cons_BigLaserBeam => AnimationID.book_idle,
                Cons_ToxicCloud => AnimationID.book_idle,

                Cons_Heal => AnimationID.gem_idle,
                Cons_TP => AnimationID.gem_idle,
                Cons_GroundSlam => AnimationID.gem_idle,

                Cons_Bomb => AnimationID.bomb_idle,
                Cons_WolfTrap => AnimationID.bomb_idle,

                _ => AnimationID.unknown
            };

            //play idle animation
            PlayAnimation(currentDefaultIdlePose);

            //play additive pickup animation
            _animator.SetTrigger(PlayPickupAnimation_AnimatorProperty);

            //link item-specific events
            switch (equippedItem)
            {
                case Crossbow crossbow:
                    const float crossbowShootAnimClipLength = .542f;
                    CrossbowAnimationSpeedMultiplier = crossbowShootAnimClipLength / (crossbow.delayBetweenShots - .05f);
                    _animator.SetFloat(CrossbowShootAnimationSpeedMultiplier_AnimatorProperty, CrossbowAnimationSpeedMultiplier);
                    crossbow.OnCrossbowShoot += PlayCrossbowShootAnimation;
                    break;
                    // case Tomahawk tomahawk:
                    //     const float tomahawkThrowAnimClipLength = 1.083f;
                    //     float tomahawkAnimationSpeedMultiplier = tomahawkThrowAnimClipLength / (tomahawk.delayBetweenShots - .05f);
                    //     _animator.SetFloat(TomahawkThrowAnimationSpeedMultiplier_AnimatorProperty,tomahawkAnimationSpeedMultiplier);
                    //     break;
            }

        }

        void OnItemUnequipped(Item item)
        {
            print("received event unequipped item : " + item.GetType());

            //unlink item-specific events
            switch (item)
            {
                case Crossbow crossbow:
                    crossbow.OnCrossbowShoot -= PlayCrossbowShootAnimation;
                    break;
            }

            currentDefaultIdlePose = AnimationID.freeHands_idle;
            PlayCurrentDefaultIdlePoseAnimation();
        }

        //helper functions
        public void PlayAnimation(AnimationID id)
        {
            if (id == AnimationID.unknown)
            {
                Debug.LogWarning("Unknown animation ! falling back to current default idle pause.");
                id = currentDefaultIdlePose;
            }
            else if (id == AnimationID.currentDefaultIdlePose)
                id = currentDefaultIdlePose;

            PlayAnimationRPC((int)id);
        }

        private void PlayCurrentDefaultIdlePoseAnimation()
        {
            PlayAnimationRPC((int)currentDefaultIdlePose);
        }

        [Rpc(SendTo.Everyone)]
        // todo : ça marchait pas à cause de cette erreur : 
        // "NetworkBehaviour index 11 was out of bounds for player character_0."
        // je l'ai remis en monobehaviour du coup, mais faudrait arriver à
        // trouver d'où ça vient pour pouvoir repliquer les anims
        private void PlayAnimationRPC(int id)
        {
            _animator.SetInteger(MainAnim_AnimatorProperty, id);
            _animator.SetTrigger(PlayMainAnimation_AnimatorProperty);
        }

        //crossbow animation
        private void PlayCrossbowShootAnimation(float chargeLevel)
        {
            PlayAnimation(AnimationID.crossbow_shootAndReload);
        }

    }
}