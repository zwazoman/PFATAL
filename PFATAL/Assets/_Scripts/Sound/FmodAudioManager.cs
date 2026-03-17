using AYellowpaper.SerializedCollections;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using FMOD;

public class FmodAudioManager : NetworkBehaviour
{
    public FmodAudioManager instance { get; private set; }

    [SerializeField] List<EventReference> eventReferences;

    List<EventInstance> eventInstances = new();

    string _soundsEnumFilePath = "Assets/_Scripts/Sound/FmodEventsEnum.cs";

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if(instance != null)
            UnityEngine.Debug.LogError("plusieurs audiomanagers dans la scene");
        instance = this;

        SceneManager.activeSceneChanged += (_,_) => CleanUp();

        LoadDictionary();
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
    public void PlayOneShotGlobalRpc(Sounds soundEvent, Vector3 pos)
    {
        //if (eventReferencesNamesDict.ContainsKey(soundEvent))
        //    PlayOneShot(soundEvent, pos);
        //else
        //    UnityEngine.Debug.LogError("sound name does not exist in event references dictionnary");
    }

    public void PlayOneShot(Sounds soundEvent, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(eventReferences[(int)soundEvent], pos);
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

#if UNITY_EDITOR

    [ContextMenu("Load Sounds Enum")]
    void LoadDictionary()
    {
        //todo génerer l'enum et ajouter tout au dico

        string enumString = "";

        foreach (EventReference reference in eventReferences)
        {
            string referenceName = reference.ToString();

            int stringStart = referenceName.IndexOf("/", 0);
            stringStart = referenceName.IndexOf('/', stringStart + 1);
            int stringEnd = referenceName.IndexOf(")");

            int stringLength = stringEnd - stringStart;

            enumString += referenceName.Substring(stringStart + 1, stringLength - 1) + ",";
        }

        GenerateSoundEnum(enumString);
    }

    void GenerateSoundEnum(string enumContent)
    {
        string enumText = "public enum Sounds{" + enumContent + "}";
        
        File.WriteAllText(_soundsEnumFilePath, enumText);
    }

#endif
}
