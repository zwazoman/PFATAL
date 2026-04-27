using UnityEngine;

namespace GameplaySystems.PlayerCharacter
{
    public class PlayerHandsVisuals : MonoBehaviour
    {
        private static readonly int EquippedItemTypeAnimatorProperty = Animator.StringToHash("equippedItemType");

        [Header("Scene references")]
        [SerializeField] Hand _hand;
        [SerializeField] Animator _animator;

        void Awake()
        {
            _hand.OnEquipItem += SetEquippedItemTypeAnimatorProperty;
            _hand.OnUnequipItem += (_) => ClearEquippedItem();
            _hand.OnDeleteItem += ClearEquippedItem;
            _hand.OnDropItem += (_) => ClearEquippedItem();
        }

        private void ClearEquippedItem()
        {
            _animator.SetInteger(EquippedItemTypeAnimatorProperty,0);
        }
        
        private void SetEquippedItemTypeAnimatorProperty(Item equippedItem)
        {
            _animator.SetInteger(EquippedItemTypeAnimatorProperty,equippedItem switch
            {
                Crossbow => 101, // 100 => weapon
                Tomahawk => 102,
                Sword => 103,
                
                Cons_Tornado => 201, // 200 => book
                Cons_BigLaserBeam => 202,
                Cons_ToxicCloud => 203,
                
                Cons_Heal => 301, // 300 => gemstone
                Cons_TP => 302,
                Cons_GroundSlam => 303,
                
                Cons_WolfTrap => 401, // 400 => other
                Cons_Bomb => 402,
                
                _ => 0
            });
        }
    }
}