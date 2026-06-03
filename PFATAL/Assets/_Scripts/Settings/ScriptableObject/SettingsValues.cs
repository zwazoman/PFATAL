using UnityEngine;

[CreateAssetMenu(fileName = "SettingsValue", menuName = "Scriptable Objects/Settings")]
public class SettingsValues : ScriptableObject
{
    [Header("Controls")]
    [Range(50f,90f)]public float FOV;
    [Range(5f, 30f)]public float SensitivityMouse;
    [Range(5f, 50f)]public float SensitivityController;

    [Header("Rebinds")]
    public float PlaceHolder;

    [Header("Graphics")]
    public float Gamma;
    public float FramesRates;
    public bool VSync;
}
