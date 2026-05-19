using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class Ragdolll : MonoBehaviour
{
    public List<GameObject> RagdollParts = new List<GameObject>();
    public GameObject newParent;
    public List<GameObject> body = new List<GameObject>();
    public Rigidbody parentRb;

    private void Start()
    {
        //ActiveRagdoll();
    }

    [Button("Activate Ragdoll")]
    public void ActiveRagdoll()
    {
        foreach (GameObject bodyPart in body)
        {
            bodyPart.SetActive(true);
        }

        foreach (GameObject part in RagdollParts)
        {
            part.SetActive(true);


            part.transform.SetParent(newParent.transform);
            part.AddComponent<Rigidbody>();
            Rigidbody rb = part.GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized/2;
            Vector3 dir = parentRb.linearVelocity.normalized + randomDirection;

            rb.AddForce(randomDirection * 10, ForceMode.Impulse);
            rb.AddTorque(randomDirection * 5, ForceMode.Impulse);
        }
    }
}
