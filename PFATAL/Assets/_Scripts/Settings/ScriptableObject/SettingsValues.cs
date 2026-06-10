using UnityEngine;

[CreateAssetMenu(fileName = "SettingsValue", menuName = "Scriptable Objects/Settings")]
public class SettingsValues : ScriptableObject
{
    [Header("Controls")]
    [Range(50f,90f)]public float FOV;
    [Range(5f, 30f)]public float SensitivityMouse;
    [Range(5f, 50f)]public float SensitivityController;

    [Header("Volumes")]
    public float MasterVolume;
    public float MusicVolume;
    public float SfxVolume;

    [Header("Rebinds")]
    public string MoveUp;
    public string MoveDown;
    public string MoveLeft;
    public string MoveRight;
    public string Jump;
    public string Interact;
    public string Drop;

    [Header("Graphics")]
    //public float Gamma;
    public int FramesRates;
    public bool VSync;
}
