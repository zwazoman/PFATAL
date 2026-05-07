#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class AnimationEventSaver : MonoBehaviour
{
    [SerializeField] Mesh _mesh;
    [SerializeField] UnityEngine.Object oui;
    [SerializeField] AnimationClip _animClip;

    [SerializeField] List<EventSave> _eventSaves = new();

    [ContextMenu("Save Events")]
    void SaveEvents()
    {
        print(oui.GetType().ToString());

        if(_animClip == null)
        {
            Debug.LogError("no clip");
            return;
        }

        foreach(AnimationEvent animEvent in _animClip.events)
        {
            EventSave save = new(_animClip.name, animEvent.time, animEvent.functionName, animEvent.stringParameter, animEvent.floatParameter, animEvent.intParameter);
            _eventSaves.Add(save);
        }
    }

    [ContextMenu("Clear Events")]
    void ClearEvents()
    {
        AnimationUtility.SetAnimationEvents(_animClip, Array.Empty<AnimationEvent>());
    }

    [ContextMenu("Restore Events")]
    void RestoreEvents()
    {
        if (_animClip == null)
        {
            Debug.LogError("no clip");
            return;
        }

        if(_animClip.events.Length != 0)
        {
            Debug.LogError("Animclip events not empty");
            return;
        }

        List<EventSave> tmpDeleteList = new();
        List<AnimationEvent> savedEventsist = new();

        foreach(EventSave save in _eventSaves)
        {
            if(save.animName == _animClip.name)
            {
                AnimationEvent newEvent = new();
                newEvent.time = save.time;
                newEvent.functionName = save.functionName;
                newEvent.stringParameter = save.stringParam;
                newEvent.intParameter = save.intParam;
                newEvent.floatParameter = save.floatParam;

                _animClip.AddEvent(newEvent);
                tmpDeleteList.Add(save);   
                savedEventsist.Add(newEvent);
            }
        }

        AssetDatabase.SaveAssetIfDirty(_animClip);
        AssetDatabase.SaveAssetIfDirty(oui);
        //AnimationUtility.SetAnimationEvents(_animClip, savedEventsist.ToArray());

        foreach (AnimationEvent connard in _animClip.events)
        {
            print(connard.functionName);
        }

        //foreach (EventSave save in tmpDeleteList)
        //{
        //    if (_eventSaves.Contains(save))
        //        _eventSaves.Remove(save);
        //}
    }
}

[Serializable]
struct EventSave
{
    public string animName;
    public float time;
    public string functionName;
    public string stringParam;
    public float floatParam;
    public int intParam;

    public EventSave(string animName, float time, string functionName, string stringParam, float floatParam, int intParam)
    {
        this.animName = animName;
        this.time = time;
        this.functionName = functionName;
        this.stringParam = stringParam;
        this.floatParam = floatParam;
        this.intParam = intParam;
    }
}

#endif
