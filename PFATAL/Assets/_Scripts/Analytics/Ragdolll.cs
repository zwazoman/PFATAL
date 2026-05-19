using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class Ragdolll : MonoBehaviour
{
    public List<GameObject> RagdollParts = new List<GameObject>();
    public GameObject newParent;

    private void Start()
    {
        //ActiveRagdoll();
    }

    [Button("Activate Ragdoll")]
    public void ActiveRagdoll()
    {
        foreach (GameObject part in RagdollParts)
        {
            part.transform.SetParent(newParent.transform);
            part.AddComponent<Rigidbody>();
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized/2;
            part.GetComponent<Rigidbody>().AddForce(randomDirection * 10, ForceMode.Impulse);
            part.GetComponent<Rigidbody>().AddTorque(randomDirection * 5, ForceMode.Impulse);
        }
    }
}
