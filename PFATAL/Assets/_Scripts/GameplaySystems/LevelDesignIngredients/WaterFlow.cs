using UnityEngine;

public class WaterFlow : MonoBehaviour
{
    [Range(0, 360)]
    public float DirectionalWaterLevel = 5f;
    public float flowSpeed = 5f;

    private Vector3 _flowDirection;

    void Start()
    {
        UpdateFlowDirection();
    }

    void OnTriggerStay(Collider collision)
    {
        if (collision.TryGetComponent(out PlayerPhysics physics))
        {
            if (physics.enabled)
            {
                physics.AddForce(_flowDirection * flowSpeed);
            }
        }
    }

    void OnDrawGizmos()
    {
        UpdateFlowDirection();
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, _flowDirection * 10f);
    }

    private void UpdateFlowDirection()
    {
        _flowDirection = Quaternion.AngleAxis(DirectionalWaterLevel, Vector3.up) * Vector3.forward;
    }
}