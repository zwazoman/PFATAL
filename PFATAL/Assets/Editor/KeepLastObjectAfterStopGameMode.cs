using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class KeepLastObjectAfterStopGameMode
{
    static KeepLastObjectAfterStopGameMode() { }

    static GameObject lastSelectedObject;

    static void PlayModeSelectionWatcher()
    {
        Debug.Log("AntiDeselect loaded");
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode && Selection.activeGameObject != null)
        {
            lastSelectedObject = Selection.activeGameObject;
        }

        if (state == PlayModeStateChange.ExitingPlayMode && lastSelectedObject != null)
        {
            Selection.activeGameObject = lastSelectedObject;
        }
    }
}