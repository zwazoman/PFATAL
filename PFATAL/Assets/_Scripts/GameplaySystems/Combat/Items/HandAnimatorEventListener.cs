using System;
using Unity.VisualScripting;
using UnityEngine;

public class HandAnimatorEventListener : MonoBehaviour
{
    [SerializeField] private Hand _hand;
    [SerializeField] private Animator _animator;
    
    
    //public event
    public event Action OnAnimationFinished;
    
    /// <summary>
    /// 0 -> small attack 0,  
    /// 1 -> small attack 1,  
    /// 2 -> dash release,  
    /// </summary>
    public event Action<int> OnSwordHitboxActivated;
    public event Action OnSwordHitboxDeactivated;
    

    
    
    
    //animator messages
    public void TriggerOnSwordHitboxActivated(int attackID) { print("TriggerOnSwordHitboxActivated"); OnSwordHitboxActivated?.Invoke(attackID); }
    public void TriggerOnSwordHitboxDeactivated(){print("TriggerOnSwordHitboxDeactivated");OnSwordHitboxDeactivated?.Invoke();}
    public void TriggerOnAnimationFinished(){print("TriggerOnAnimationFinished");OnAnimationFinished?.Invoke();}
}
