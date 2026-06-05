using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// reçoit et propage des events depuis les animations de la main
/// </summary>
public class HandAnimatorEventListener : MonoBehaviour
{
    [SerializeField] private Hand _hand;
    [SerializeField] private Animator _animator;
    
    //public event
    public event Action OnAnimationFinished;
    public event Action OnSwordHitboxActivated;
    public event Action OnSwordHitboxDeactivated;
    public event Action OnSwordAttackStart;
    public event Action OnGrapplePulled;
    public event Action OnTomahawkShot;
    public event Action OnGemBroken;
    public event Action OnSpellCast;
    public event Action OnObjectThrown;
    public event Action OnFirstAttackFinished;
    
    
// == animator messages ==
    
    //general
    public void TriggerOnAnimationFinished(){print("TriggerOnAnimationFinished");OnAnimationFinished?.Invoke();}
    
    //sword
    public void TriggerOnSwordHitboxActivated(){print("TriggerOnSwordHitboxActivated"); OnSwordHitboxActivated?.Invoke(); }
    public void TriggerOnSwordHitboxDeactivated(){print("TriggerOnSwordHitboxDeactivated");OnSwordHitboxDeactivated?.Invoke();}
    public void TriggerOnFirstAttackFinished(){print("TriggerOnFirstAttackFinished"); OnFirstAttackFinished?.Invoke();}
    
    //tomahawk
    public void TriggerOnGrapplePulled(){print("TriggerOnGrapplePulled");OnGrapplePulled?.Invoke();}
    public void TriggerOnTomahawkShot(){print("TriggerOnTomahawkShot");OnTomahawkShot?.Invoke();}
    
    //items
    public void TriggerOnGemBreak() { print("TriggerOnGemBreak"); OnGemBroken?.Invoke(); }
    public void TriggerOnSpellCast() { print("TriggerOnSpellCast"); OnSpellCast?.Invoke(); }
    public void TriggerOnObjectThrown() { print("TriggerOnObjectThrown"); OnObjectThrown?.Invoke(); }
}
