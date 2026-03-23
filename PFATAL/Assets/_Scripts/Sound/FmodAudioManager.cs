using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class FmodAudioManager : NetworkBehaviour
{
    #region Singleton
    private static FmodAudioManager instance;

    public static FmodAudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("no instance of audiomanager");
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

    [SerializeField] List<EventReference> eventReferences;

    List<EventInstance> eventInstances = new();

    string _soundsEnumFilePath = "Assets/_Scripts/Sound/FmodEventsEnum.cs";

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        PlayOneShot(Sounds.Music);
    }

    public void PlayOneShot(Sounds sound, Vector3 pos = default)
    {
        try
        {
            RuntimeManager.PlayOneShot(GetReference(sound), pos);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    [Rpc(SendTo.NotMe)]
    public void PlayOneShotForOthersRPC(Sounds sound, Vector3 pos = default)
    {
        PlayOneShot(sound, pos);
    }

    public EventInstance CreateInstance(Sounds sound)
    {
        EventInstance instance = RuntimeManager.CreateInstance(GetReference(sound));
        eventInstances.Add(instance);

        return instance;
    }

    public void CleanUp()
    {
        foreach (EventInstance instance in eventInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    EventReference GetReference(Sounds sound)
    {
        if (eventReferences[(int)sound].IsNull)
            Debug.LogError($"sound {sound.ToString()} does not exist");

        return eventReferences[(int)sound];
    }

#if UNITY_EDITOR

    [ContextMenu("Load Sounds Enum")]
    void LoadDictionary()
    {
        string enumString = "";

        foreach (EventReference reference in eventReferences)
        {
            string referenceName = reference.ToString();

            string referenceSub = referenceName;

            print(referenceSub);

            while (referenceSub.Contains("/"))
            {
                int stringStart = referenceSub.IndexOf("/") + 1;
                int stringEnd = referenceSub.Length - 1;

                int stringLength = (stringEnd - stringStart) + 1;

                print($"{stringStart} {stringEnd} {stringLength}");

                referenceSub = referenceSub.Substring(stringStart, stringLength);
            }

            referenceSub = referenceSub.Substring(0, referenceSub.Length - 1);
            print(referenceSub);

            enumString += referenceSub + ",";
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
