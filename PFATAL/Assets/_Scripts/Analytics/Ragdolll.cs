using _scripts.PlayerCharacter;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class Ragdolll : MonoBehaviour
{
    [SerializeField] private List<GameObject> RagdollParts = new List<GameObject>();
    [SerializeField] private GameObject newParent;
    [SerializeField] private List<GameObject> body = new List<GameObject>();
    [SerializeField] private Rigidbody parentRb;

    private List<Vector3> initialPositions = new List<Vector3>();
    private List<Quaternion> initialRotations = new List<Quaternion>();

    private void Start()
    {
        //ActiveRagdoll();

        foreach (GameObject part in RagdollParts)
        {
            initialPositions.Add(part.transform.localPosition);
            initialRotations.Add(part.transform.localRotation);
        }

        parentRb.GetComponent<PlayerCharacterNetworkStateMachineCallback>().OnStateChanged += (oldState, newState) =>
        {
            if (newState == PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum.Dead /*HasFlag(PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum.Dead)*/)
            {
                ActiveRagdoll();
            }
        };

        //parentRb.GetComponent<PlayerCharacter>().inputs.OnRespawnInput
    }

    [Button("Activate Ragdoll", EButtonEnableMode.Playmode)]
    public void ActiveRagdoll()
    {
        foreach (GameObject bodyPart in body)
        {
            bodyPart.SetActive(true);
        }

        foreach (GameObject part in RagdollParts)
        {
            part.SetActive(true);


            part.TryGetComponent(out Rigidbody rb);

            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized/2;
            Vector3 dir = parentRb.linearVelocity.normalized + randomDirection;

            rb.isKinematic = false;
            rb.AddForce(randomDirection * 10, ForceMode.Impulse);
            rb.AddTorque(randomDirection * 5, ForceMode.Impulse);
        } 
    }

    [Button("Reset Ragdoll", EButtonEnableMode.Playmode)]
    public void Reset()
    {
        /*foreach (GameObject part in RagdollParts)
        {
            part.transform.localPosition = initialPositions[RagdollParts.IndexOf(part)];
            part.transform.localRotation = initialRotations[RagdollParts.IndexOf(part)];
        }*/

        for (int i = 0; i < RagdollParts.Count; i++)
        {
            RagdollParts[i].TryGetComponent(out Rigidbody rb);
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            RagdollParts[i].transform.localPosition = initialPositions[i];
            RagdollParts[i].transform.localRotation = initialRotations[i];
        }
    }
}
