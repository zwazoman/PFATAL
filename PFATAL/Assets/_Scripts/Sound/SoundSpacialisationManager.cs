using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundSpacialisationManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] AudioManager _audiomanager;

    [Header("Settings")]
    [SerializeField] LayerMask _occlusionLayerMask;

    [Header("Occlusion Settings")]
    [SerializeField] float _thinWallOcclusionValue = .1f;
    [SerializeField] float _mediumWallOcclusionValue = .3f;
    [SerializeField] float _ThickWallOcclusionValue = .5f;

    [Header("Reverb Detection Stetings")]
    [SerializeField] float _sphereRadius = .1f;

    [Header("Settings")]
    [SerializeField] LayerMask _reverbZoneLayerMask;

    [SerializeField] StudioListener _listener;
    StudioEventEmitter _emitter;

    bool _gameStarted;

    float _timer;

    private void Start()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.EventOnGameStarted += GameStarted_Callback;

        _audiomanager.On3DSoundPlayed += ApplyOcclusion;
        _audiomanager.On3DSoundPlayed += ApplyReverb;
    }

    void GameStarted_Callback()
    {
        print("gameStarted");
    }

    private void Update()
    {
        if (GameManager.Instance != null)
            _listener = GameManager.Instance.localPlayerCharacter.listener;

        if (_listener == null)
            return;

        //update delay
        if (_timer < AudioManager.TIME_BETWEEN_REVERB_OCCLUSION_CHECKS)
        {
            _timer += Time.deltaTime;
            return;
        }
        _timer = 0;
            

        List<EventInstance> stoppedEvents = new();

        foreach(EventInstance instance in _audiomanager.EventInstances3D)
        {
            if(!instance.isValid())
            {
                stoppedEvents.Add(instance);
                continue;
            }

            ApplyReverb(instance);
            ApplyOcclusion(instance);
        }

        foreach(EventInstance eventInstance in stoppedEvents)
        {
            _audiomanager.EventInstances3D.Remove(eventInstance);
        }
        stoppedEvents.Clear();

    }

    void ApplyReverb(EventInstance instance)
    {
        print("reverb");
        Collider[] colliders = new Collider[1];

        Physics.OverlapSphereNonAlloc(GetInstancePos(instance), _sphereRadius, colliders, _reverbZoneLayerMask);

        if (colliders[0] != null)
        {
            //todo => récupérer le tag pour pouvoir set des reverbs différentes
            print($"apply la reverb sur {instance}");
            instance.setParameterByName("ReverbAmount", 1);
        }
        else
            instance.setParameterByName("ReverbAmount", 0);
    }

    void ApplyOcclusion(EventInstance instance)
    {
        float occlusionValue = 0;

        Vector3 instanceListenerOffset = GetInstancePos(instance) - _listener.transform.position;

        RaycastHit[] hits = Physics.RaycastAll(_listener.transform.position, instanceListenerOffset, instanceListenerOffset.magnitude, _occlusionLayerMask);

        if (hits.Length > 0)
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("ThinWall"))
                    occlusionValue += _thinWallOcclusionValue;
                else if (hit.collider.CompareTag("MediumWall"))
                    occlusionValue += _mediumWallOcclusionValue;
                else if (hit.collider.CompareTag("ThickWall"))
                    occlusionValue += _ThickWallOcclusionValue;
            }

        occlusionValue = Mathf.Clamp01(occlusionValue);

        instance.setParameterByName("Occlusion", occlusionValue);
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
