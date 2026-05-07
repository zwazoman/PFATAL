using Unity.VisualScripting;
using UnityEngine;

namespace GameplaySystems.PlayerCharacter
{
    public class PlayerHandVisuals : MonoBehaviour
    {

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
            sword_attack_small_0 = 301,
            sword_attack_small_1 = 302,
            sword_charge_release = 303,
            sword_charge_idle = 304,
            
            book_idle = 400,
            book_use = 401,
            
            gem_idle = 500,
            gem_break = 501,
            
            bomb_idle = 600,
            bomb_use = 601,
        }
        
        
        private static readonly int MainAnim_AnimatorProperty = Animator.StringToHash("mainAnim");
        private static readonly int CrossbowShootAnimationSpeedMultiplier_AnimatorProperty = Animator.StringToHash("crossbowShootAnimationSpeedMultiplier");

        [Header("Scene references")]
        [SerializeField] Hand _hand;
        [SerializeField] Animator _animator;

        private AnimationID currentDefaultIdlePose;
        
        void Awake()
        {
            _hand.OnEquipItem += OnNewItemEquipped;
            _hand.OnUnequipItem += OnItemUnequipped;
            _hand.OnDeleteItem += PlayCurrentDefaultIdlePoseAnimation;
            _hand.OnDropItem += OnItemUnequipped;
            _hand.animatorEventListener.OnAnimationFinished += PlayCurrentDefaultIdlePoseAnimation;

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
                //todo : Cons_WolfTrap ,
                
                _ => AnimationID.unknown
            };
            PlayAnimation(currentDefaultIdlePose);
            
            //link events
            switch (equippedItem)
            {
                case Crossbow crossbow:
                    const float crossbowShootAnimClipLength = .542f;
                    _animator.SetFloat(CrossbowShootAnimationSpeedMultiplier_AnimatorProperty,crossbowShootAnimClipLength/(crossbow.delayBetweenShots-.05f));
                    crossbow.OnCrossbowShoot += PlayCrossbowShootAnimation;
                    break;
            }
            
        }
        
        void OnItemUnequipped(Item item)
        {
            //unlink events
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
            
            _animator.SetInteger(MainAnim_AnimatorProperty, (int)id);
        }

        void PlayCurrentDefaultIdlePoseAnimation()
        {
            _animator.SetInteger(MainAnim_AnimatorProperty, (int)currentDefaultIdlePose);
        }
        
        //sword animation
        private bool _swordAnimFlipFlop;
        public void PlaySwordAttackAnimation()
        {
            PlayAnimation(_swordAnimFlipFlop ? AnimationID.sword_attack_small_0 : AnimationID.sword_attack_small_1);
            _swordAnimFlipFlop = !_swordAnimFlipFlop;
        }
        
        //crossbow animation
        private void PlayCrossbowShootAnimation(float chargeLevel)
        {
            PlayAnimation(AnimationID.crossbow_shootAndReload);
        }

    }
}