using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float strength;
    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerPhysics physics))
        {
            if(physics.enabled) // TODO : faire un produit scalaire
            {
                Vector3 newVelocity = Vector3.ProjectOnPlane(physics.Velocity, transform.up);
                physics.SetVelocity(newVelocity + transform.up * strength);
            }

        }
        // else if (collider.TryGetComponent(out Rigidbody rb))
        // {
        //     
        // }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.up * strength);
    }
}
