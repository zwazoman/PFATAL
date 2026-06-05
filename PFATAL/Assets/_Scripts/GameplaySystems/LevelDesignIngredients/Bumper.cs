using System;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    private static readonly int JumpProperty = Animator.StringToHash("jump");
    [SerializeField] Animator _animator;
    public event Action OnPlayerBounce;
    public float strength;
    
    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerPhysics physics))
        {
            if(physics.enabled) 
            {
                //add impulse to player character
                OnPlayerBounce?.Invoke();
                Vector3 newVelocity = Vector3.ProjectOnPlane(physics.Velocity, transform.up);
                physics.SetVelocity(newVelocity + transform.up * strength);
            }

            //play anim
            _animator.SetTrigger(JumpProperty);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.up * strength);
    }
}
