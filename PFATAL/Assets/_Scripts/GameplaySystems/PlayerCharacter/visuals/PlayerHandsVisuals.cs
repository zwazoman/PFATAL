using UnityEngine;

namespace GameplaySystems.PlayerCharacter
{
    public class PlayerHandsVisuals : MonoBehaviour
    {
        private static readonly int Anim_AnimatorProperty = Animator.StringToHash("anim");

        [Header("Scene references")]
        [SerializeField] Hand _hand;
        [SerializeField] Animator _animator;

        private int currentIdleID;
        
        void Awake()
        {
            _hand.OnEquipItem += SetEquippedItemTypeAnimatorProperty;
            _hand.OnUnequipItem += (_) => ClearEquippedItem();
            _hand.OnDeleteItem += ClearEquippedItem;
            _hand.OnDropItem += (_) => ClearEquippedItem();
        }

        private void ClearEquippedItem()
        {
            _animator.SetInteger(Anim_AnimatorProperty,0);
        }
        
        private void SetEquippedItemTypeAnimatorProperty(Item equippedItem)
        {
            _animator.SetInteger(Anim_AnimatorProperty,equippedItem switch
            {
                Crossbow => 100, //100 => crossbow
                Tomahawk => 200, //200 => Tomahawk
                Sword => 300,    //300 => Sword
                
                Cons_Tornado => 400,      //400 => book
                Cons_BigLaserBeam => 400,
                Cons_ToxicCloud => 400,
                
                Cons_Heal => 500, // 500 => gemstone
                Cons_TP => 500,
                Cons_GroundSlam => 500,
                
                Cons_Bomb => 600, //600 => Bomb
                Cons_WolfTrap => 700,
                
                _ => 0
            });
        }
    }
}