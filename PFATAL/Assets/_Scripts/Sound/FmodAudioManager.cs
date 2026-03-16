using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FmodAudioManager : NetworkBehaviour
{
    public FmodAudioManager instance { get; private set; }

    List<EventInstance> eventInstances = new();

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if(instance != null)
            Debug.LogError("plusieurs audiomanagers dans la scene");
        instance = this;

        SceneManager.activeSceneChanged += (_,_) => CleanUp();
    }

    //todo : rpc faut tout link au network. y'aura surement besoin d'fair eun dictionnaire avec des string

    public void PlayOneShot(EventReference sound, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(sound, pos);
    }

    public EventInstance CreateInstance(EventReference sound)
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        eventInstances.Add(instance);

        return instance;
    }

    public void CleanUp()
    {
        foreach(EventInstance instance in eventInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }
}
