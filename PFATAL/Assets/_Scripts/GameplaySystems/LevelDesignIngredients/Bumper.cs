using System;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public event Action OnPlayerBounce;

    public float strength;
    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerPhysics physics))
        {
            if(physics.enabled) // TODO : faire un produit scalaire
            {
                OnPlayerBounce?.Invoke();
                Vector3 newVelocity = Vector3.ProjectOnPlane(physics.Velocity, transform.up);
                physics.SetVelocity(newVelocity + transform.up * strength);
            }

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.up * strength);
    }
}
