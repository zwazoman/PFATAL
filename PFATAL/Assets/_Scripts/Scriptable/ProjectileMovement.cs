using UnityEngine;

[CreateAssetMenu(fileName = "new ProjectileMovement", menuName = "Scriptable Objects/ProjectileMovement")]
public class ProjectileMovement : ScriptableObject
{
    [Tooltip("Vitesse horizontale (unités/s)")]
    public float speed = 20f;

    [Tooltip("Accélération gravitationnelle vers le bas (unités/s²)")]
    public float gravity = 10f;

    public Vector3 ComputePosition(float timeSinceSpawn, Vector3 startPos, Vector3 startDir)
    {
        return startPos
             + startDir.normalized * (speed * timeSinceSpawn)
             + Vector3.up * (-0.5f * gravity * timeSinceSpawn * timeSinceSpawn);
    }
}