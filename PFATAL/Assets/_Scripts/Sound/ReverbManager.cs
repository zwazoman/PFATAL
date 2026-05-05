using FMOD;
using FMOD.Studio;
using UnityEngine;

public class ReverbManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioManager _audioManager;

    [Header("Settings")]
    [SerializeField] LayerMask _reverbZoneLayerMask;

    [Header("Reverb Detection Stetings")]
    [SerializeField] float _sphereRadius = .1f;

    private void Start()
    {
        _audioManager.On3DSoundPlayed += ApplyReverb;
    }

    void ApplyReverb(EventInstance instance)
    {
        Collider[] colliders = new Collider[1];

        Physics.OverlapSphereNonAlloc(GetInstancePos(instance), _sphereRadius, colliders, _reverbZoneLayerMask);

        if (colliders[0] != null)
        {
            //todo => récupérer le tag pour pouvoir set des reverbs différentes

            instance.setParameterByName("ReverbAmount", 1);
        }
    }

    Vector3 GetInstancePos(EventInstance instance)
    {
        instance.get3DAttributes(out ATTRIBUTES_3D attributes);

        Vector3 instancePos = new Vector3(
            attributes.position.x,
            attributes.position.y,
            attributes.position.z
            );

        return instancePos;
    }
}
