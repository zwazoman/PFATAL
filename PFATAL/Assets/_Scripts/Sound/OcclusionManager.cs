using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class OcclusionManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Audiomanager _audiomanager;

    [Header("Settings")]
    [SerializeField] LayerMask _occlusionLayerMask;

    StudioListener _listener;
    StudioEventEmitter _emitter;

    private void Start()
    {
        GameManager.Instance.EventOnGameStarted += GameStarted_Callback;
    }

    void GameStarted_Callback()
    {
        _listener = GameManager.Instance.GetPlayerCharacter(OwnerClientId).listener;
    }

    void ApplyOcclusion(EventInstance instance)
    {
        float occlusionValue = 0;
        instance.get3DAttributes(out ATTRIBUTES_3D attributes);

        Vector3 instancePos = new Vector3(
            attributes.position.x,
            attributes.position.y,
            attributes.position.z
            );

        Vector3 instanceListenerOffset = instancePos - _listener.transform.position;

        RaycastHit[] hits = Physics.RaycastAll(_listener.transform.position, instanceListenerOffset, instanceListenerOffset.magnitude, _occlusionLayerMask);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Thin"))
                print("thin wall hit");
            else if (hit.collider.CompareTag("Medium"))
                print("medium wall hit");
            else if (hit.collider.CompareTag("Thick"))
                print("thick wall hit");
            else
            {
                print("wall hit");
            }

            print($"there were {hits.Length} walls between the source and the listener");
        }

        //instance.setParameterByName("Occlusion", occlusionValue);
    }

}
