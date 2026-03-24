using FMOD.Studio;
using FMODUnity;
using Unity.Netcode;
using UnityEngine;

public class OcclusionManager : NetworkBehaviour
{
    [SerializeField] FmodAudioManager _audiomanager;
    StudioListener _listener;

    private void Start()
    {
        GameManager.Instance.EventOnGameStarted += GameStarted_Callback;
    }

    void GameStarted_Callback()
    {
        _listener = GameManager.Instance.GetPlayerCharacter(OwnerClientId).listener;
    }

    void ApplyOcclusion(EventInstance instance, Vector3 pos)
    {
        float occlusionValue = 0;

        instance.setParameterByName("Occlusion", occlusionValue);
    }

}
