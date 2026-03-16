using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float strength;
    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerPhysics physics))
        {
            if(physics.enabled) physics.AddImpulse(transform.up*strength); // TODO : faire un produit scalaire
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
