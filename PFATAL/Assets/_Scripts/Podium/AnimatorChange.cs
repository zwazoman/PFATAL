using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AnimatorChange : MonoBehaviour
{
    [SerializeField] bool _animation;
    [SerializeField] string _animationName;
    [SerializeField] float _delay;
    [SerializeField] float _speed;
    
    Animator _animator;

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(PlayAnim());
    }

    private IEnumerator PlayAnim()
    {
        yield return  new WaitForSeconds(_delay);
        _animator.SetBool(_animationName, _animation);
        _animator.speed = _speed;
    }
}
