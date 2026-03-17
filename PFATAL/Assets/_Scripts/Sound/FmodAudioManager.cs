using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FmodAudioManager : NetworkBehaviour
{
    public FmodAudioManager instance { get; private set; }

    [SerializeField] List<EventReference> eventReferences;

    Dictionary<string, EventReference> eventReferencesNamesDict = new();

    List<EventInstance> eventInstances = new();

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if(instance != null)
            Debug.LogError("plusieurs audiomanagers dans la scene");
        instance = this;

        SceneManager.activeSceneChanged += (_,_) => CleanUp();

        foreach(EventReference reference in eventReferences)
        {
            eventReferencesNamesDict.Add(reference.ToString(), reference);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    /// <summary>
    /// joue un son sur chaque client. a utiliser avec grande parcimonie car le délai peut être assez mauvais
    /// </summary>
    /// <param name="soundEventName"></param>
    /// <param name="pos"></param>
    [Rpc(SendTo.Everyone)]
    public void PlayOneShotGlobalRpc(string soundEventName, Vector3 pos)
    {
        if (eventReferencesNamesDict.ContainsKey(soundEventName))
            PlayOneShot(eventReferencesNamesDict[soundEventName], pos);
        else
            Debug.LogError("sound name does not exist in event references dictionnary");
    }

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
