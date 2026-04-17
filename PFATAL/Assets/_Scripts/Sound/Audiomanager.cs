using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class AudioManager : NetworkBehaviour
{
    #region Singleton
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("no audiomanager in the scene");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
        SceneManager.activeSceneChanged += (_, _) => CleanUp();
    }
    #endregion

    public event Action<EventInstance> On3DSoundPlayed;

    public bool playSounds = false;

    public List<EventInstance> EventInstances3D = new();

    [SerializeField] List<EventReference> _eventReferences;
    List<EventInstance> _eventInstances = new();

    string _soundsEnumFilePath = "Assets/_Scripts/Sound/FmodEventsEnum.cs";

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //PlayOneShot(Sounds.Music);
    }

    public void PlayOnlineOneShots(Sounds sound2D, Sounds sound3D, Vector3 pos = default, string parameter = null, float parameterValue = 0)
    {
        PlayOneShot(sound2D, default, parameter, parameterValue);
        PlayOneShotForOthersRPC(sound3D, pos, parameter, parameterValue);
    }

    public void PlayOneShot(Sounds sound, string parameter = null, float parameterValue = 0)
    {
        PlayOneShot(sound, default, parameter, parameterValue);
    }

    public EventInstance PlayOneShot(Sounds sound, Vector3 pos, string parameter = null, float parameterValue = 0)
    {
        EventInstance newInstance = CreateInstance(sound, true);

        if (parameter != null)
        {
            if (parameterValue == MathF.Floor(parameterValue))
                newInstance.setParameterByName(parameter, (int)parameterValue);
            else
                newInstance.setParameterByName(parameter, parameterValue);
        }

        if(pos  != default)
        {
            newInstance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));

            //if (attachedObject != null)
            //    RuntimeManager.AttachInstanceToGameObject(newInstance, attachedObject);

            On3DSoundPlayed?.Invoke(newInstance);
        }

        newInstance.start();
        newInstance.release();

        return newInstance;
    }

    public EventInstance CreateInstance(Sounds sound, bool is3D = false)
    {
        EventInstance instance = RuntimeManager.CreateInstance(GetEventReference(sound));
        _eventInstances.Add(instance);

        if (is3D)
            EventInstances3D.Add(instance);

        return instance;
    }

    EventReference GetEventReference(Sounds sound)
    {
        if (_eventReferences[(int)sound].IsNull)
            Debug.LogError($"sound {sound.ToString()} does not exist");

        return _eventReferences[(int)sound];
    }

    public void CleanUp()
    {
        foreach (EventInstance instance in _eventInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    //RPCS

    [Rpc(SendTo.Everyone)]
    public void PlayOneShotForEveryoneRPC(Sounds sound, Vector3 pos = default, string parameter = null, float parameterValue = 0)
    {
        PlayOneShot(sound, pos, parameter, parameterValue);
    }

    [Rpc(SendTo.NotMe)]
    void PlayOneShotForOthersRPC(Sounds sound, Vector3 pos = default, string parameter = null, float parameterValue = 0)
    {
        PlayOneShot(sound, pos, parameter, parameterValue);
    }


#if UNITY_EDITOR

    [ContextMenu("Load Event References")]
    void LoadEventReferences()
    {
        if (_eventReferences.Count != 0)
            _eventReferences.Clear();

        foreach (EditorEventRef reference in EventManager.Events)
        {
            EventReference newReference = new();
            newReference.Guid = reference.Guid;
            newReference.Path = reference.Path;

            _eventReferences.Add(newReference);
        }
    }

    [ContextMenu("Load Sounds Enum")]
    void LoadDictionary()
    {
        string enumString = "";

        foreach (EventReference reference in _eventReferences)
        {
            string referenceName = reference.ToString();

            string referenceSub = referenceName;

            while (referenceSub.Contains("/"))
            {
                int stringStart = referenceSub.IndexOf("/") + 1;
                int stringEnd = referenceSub.Length - 1;

                int stringLength = (stringEnd - stringStart) + 1;

                referenceSub = referenceSub.Substring(stringStart, stringLength);
            }

            referenceSub = referenceSub.Substring(0, referenceSub.Length - 1);

            enumString += referenceSub + ",";
        }

        GenerateSoundEnum(enumString);
    }

    [ContextMenu("Both")]
    void Both()
    {
        LoadEventReferences();
        LoadDictionary();
    }

    void GenerateSoundEnum(string enumContent)
    {
        string enumText = "public enum Sounds{" + enumContent + "}";

        File.WriteAllText(_soundsEnumFilePath, enumText);
    }

#endif
}
