using UnityEngine;

public class WaterFlow : MonoBehaviour
{
    [Range(0, 360)]
    public float directionalWaterLevel = 5f;
    
    public float minSpeedMultiplier = 0.75f;
    public float maxSpeedMultiplier = 1.1f;
    public float flowSpeed = 20f;

    private Vector3 _flowDirection;

    private void Start()
    {
        UpdateFlowDirection();
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.TryGetComponent(out PlayerPhysics physics))
        {
            if (physics.enabled)
            {
                Vector3 playerVelocity = physics.Velocity; 
                Vector3 playerDir = new Vector3(playerVelocity.x, 0f, playerVelocity.z).normalized;
                
                var dot = Vector3.Dot(playerDir, _flowDirection);
                
                var speedMultiplier = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, (dot + 1f));

                physics.AddForce(_flowDirection * flowSpeed * speedMultiplier);
            }
        }
    }

    private void OnDrawGizmos()
    {
        UpdateFlowDirection();
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, _flowDirection * 5f);
    }

    private void UpdateFlowDirection()
    {
        _flowDirection = Quaternion.AngleAxis(directionalWaterLevel, Vector3.up) * Vector3.forward;
    }
}