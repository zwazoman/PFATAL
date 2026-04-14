using UnityEngine;

[CreateAssetMenu(fileName = "AimAssistVersions", menuName = "Scriptable Objects/AimAssistVersions")]
public class AimAssistVersions : ScriptableObject
{
    [Tooltip("The max tangent between your forward and your direction to the target.")]
    [Range(0f, 1f)] public float MaxOffset;
    [Tooltip("How much should the camera assist.")]
    [Range(0f, 1f)] public float AssistStrength;
    [Tooltip("How much the player can move the camera away.")]
    [Range(0f, 0.2f)] public float MinInputMultiplier;
}
