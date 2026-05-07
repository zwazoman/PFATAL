using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using Unity.VisualScripting;
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
            //if (instance == null)
            //{
            //    Debug.LogError("no audiomanager in the scene");
            //}
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (_, _) => CleanUp();
    }
    #endregion

    public const float TIME_BETWEEN_REVERB_OCCLUSION_CHECKS = .2f;

    public event Action<EventInstance> On3DSoundPlayed;
    public List<EventInstance> EventInstances3D = new();


    [Header("Settings")]
    public bool playSounds = false;

    [Header("Sounds")]
    [SerializeField] List<EventReference> _eventReferences;
    List<EventInstance> _eventInstances = new();

    string _soundsEnumFilePath = "Assets/_Scripts/Sound/FmodEventsEnum.cs";

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
        if (!playSounds)
            return default;

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

            On3DSoundPlayed?.Invoke(newInstance);
        }

        newInstance.start();
        newInstance.release();

        return newInstance;
    }

    public EventInstance CreateInstance(Sounds sound, bool is3D = false, bool disabledWithScene = true)
    {
        EventInstance instance = RuntimeManager.CreateInstance(GetEventReference(sound));

        if (disabledWithScene)
        {
            _eventInstances.Add(instance);

            if (is3D)
                EventInstances3D.Add(instance);
        }

        return instance;
    }

    EventReference GetEventReference(Sounds sound)
    {
        if (_eventReferences[(int)sound].IsNull)
            Debug.LogError($"sound {sound.ToString()} does not exist");

        return _eventReferences[(int)sound];
    }

    public void Trigger3dSoundPlayed(EventInstance instance)
    {
        On3DSoundPlayed(instance);
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
