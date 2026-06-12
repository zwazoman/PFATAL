using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AnimatorChange : MonoBehaviour
{
    [SerializeField] bool _animation;
    [SerializeField] string _animationName;
    
    Animator _animator;

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool(_animationName, _animation);
    }
}
