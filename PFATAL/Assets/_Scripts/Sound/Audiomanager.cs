using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Audiomanager : NetworkBehaviour
{
    public event Action<Vector3> OnOneShotSoundPlayed;
    public event Action<Vector3, EventInstance> OnInstanceSoundPlayed;

    #region Singleton
    private static Audiomanager instance;

    public static Audiomanager Instance
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

    [SerializeField] List<EventReference> _eventReferences;

    List<EventInstance> _eventInstances = new();

    string _soundsEnumFilePath = "Assets/_Scripts/Sound/FmodEventsEnum.cs";

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        PlayOneShot(Sounds.Music);
    }

    public void PlayOnlineOneShots(Sounds sound2D, Sounds sound3D, Vector3 pos = default, ulong playerClientID = 1000)
    {
        PlayOneShot(sound2D);
        PlayOneShotForOthersRPC(sound3D, pos, playerClientID);
    }

    public void PlayOneShot(Sounds sound, Vector3 pos = default, GameObject attachedObject = null)
    {
        OnOneShotSoundPlayed?.Invoke(pos);

        try
        {
            if (attachedObject != null)
                RuntimeManager.PlayOneShotAttached(GetEventReference(sound), attachedObject);
            else
                RuntimeManager.PlayOneShot(GetEventReference(sound), pos);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public EventInstance CreateInstance(Sounds sound)
    {
        EventInstance instance = RuntimeManager.CreateInstance(GetEventReference(sound));
        _eventInstances.Add(instance);

        return instance;
    }

    public void CleanUp()
    {
        foreach (EventInstance instance in _eventInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    EventReference GetEventReference(Sounds sound)
    {
        if (_eventReferences[(int)sound].IsNull)
            Debug.LogError($"sound {sound.ToString()} does not exist");

        return _eventReferences[(int)sound];
    }

    void ApplyOcclusion()
    {

    }

    //RPCS

    [Rpc(SendTo.Server)]
    public void PlayOneShotForEveryoneRPC(Sounds sound, Vector3 pos = default)
    {
        PlayOneShot(sound, pos);
    }

    [Rpc(SendTo.NotMe)]
    public void PlayOneShotForOthersRPC(Sounds sound, Vector3 pos = default, ulong followPlayerId = 1000)
    {
        if (followPlayerId != 1000)
            PlayOneShot(sound, pos, GameManager.Instance.GetPlayerCharacter(followPlayerId).gameObject);
        else
            PlayOneShot(sound, pos);
    }


#if UNITY_EDITOR

    [ContextMenu("Load Event References")]
    void LoadEventReferences()
    {
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
