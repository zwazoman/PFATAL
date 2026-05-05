using System;
using Unity.VisualScripting;
using UnityEngine;

public class HandAnimatorEventListener : MonoBehaviour
{
    [SerializeField] private Hand _hand;
    [SerializeField] private Animator _animator;
    
    public event Action OnSwordHitboxActivated;
    public event Action OnSwordHitboxDeactivated;
    
    public event Action OnAnimationFinished;

    public void TriggerOnSwordHitboxActivated(){OnSwordHitboxActivated?.Invoke();}
    public void TriggerOnSwordHitboxDeactivated(){OnSwordHitboxDeactivated?.Invoke();}
    public void TriggerOnAnimationFinished(){OnAnimationFinished?.Invoke();}
}
