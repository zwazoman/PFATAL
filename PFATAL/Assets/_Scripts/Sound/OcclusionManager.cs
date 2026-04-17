using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OcclusionManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] AudioManager _audiomanager;

    [Header("Settings")]
    [SerializeField] LayerMask _occlusionLayerMask;
    [SerializeField] float _occlusionUpdateDelay = .1f;

    [Header("Occlusion Settings")]
    [SerializeField] float _thinWallOcclusionValue = .1f;
    [SerializeField] float _mediumWallOcclusionValue = .3f;
    [SerializeField] float _ThickWallOcclusionValue = .5f;

    [SerializeField] StudioListener _listener;
    StudioEventEmitter _emitter;

    //todo retirer cette merde
    bool _tmp_hasListener;

    float _timer;

    private void Start()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.EventOnGameStarted += GameStarted_Callback;

        _audiomanager.On3DSoundPlayed += ApplyOcclusion;
    }

    void GameStarted_Callback()
    {
        _listener = GameManager.Instance.localPlayerCharacter.listener;
    }

    private void Update()
    {
        if (_listener == null)
            _listener = FindAnyObjectByType<StudioListener>();

        //update delay
        if(_timer < _occlusionUpdateDelay)
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

            ApplyOcclusion(instance);
        }

        foreach(EventInstance eventInstance in stoppedEvents)
        {
            _audiomanager.EventInstances3D.Remove(eventInstance);
        }
        stoppedEvents.Clear();

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
