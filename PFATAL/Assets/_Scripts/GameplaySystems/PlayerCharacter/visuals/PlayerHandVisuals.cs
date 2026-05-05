using UnityEngine;

namespace GameplaySystems.PlayerCharacter
{
    public class PlayerHandVisuals : MonoBehaviour
    {
        private static readonly int MainAnim_AnimatorProperty = Animator.StringToHash("mainAnim");

        [Header("Scene references")]
        [SerializeField] Hand _hand;
        [SerializeField] Animator _animator;

        private int currentIdleID;
        
        void Awake()
        {
            _hand.OnEquipItem += OnNewItemEquipped;
            _hand.OnUnequipItem += (_) => ClearEquippedItem();
            _hand.OnDeleteItem += ClearEquippedItem;
            _hand.OnDropItem += (_) => ClearEquippedItem();
        }

        private void ClearEquippedItem()
        {
            _animator.SetInteger(MainAnim_AnimatorProperty,0);
        }
        
        
        
        private void OnNewItemEquipped(Item equippedItem)
        {
            //set idle pose
            currentIdleID = equippedItem switch
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
                //todo : Cons_WolfTrap => 700,
                
                _ => 0
            };
            _animator.SetInteger(MainAnim_AnimatorProperty, currentIdleID);
            
            //setup event
            switch (equippedItem)
            {
                case Sword sword:
                    sword.OnSmallAttackStarted += ()=> _animator.SetInteger(MainAnim_AnimatorProperty,301);
                    sword.OnStartCharging += () => print("OnStartCharging");
                    sword.OnDashStarted += () => print("OnStopCharging");
                    sword.OnDashCooledUp += () => print("OnDashCooledUp");
                    break;
                
                default:
                    //throw new System.NotImplementedException();
                    break;
            }
            
        }
    }
}