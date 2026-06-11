using UnityEngine;

public class NoisePosition : MonoBehaviour
{
    
    [SerializeField]private float radius = 20;
    [SerializeField]private float smoothSpeed = .2f;
    
    private Vector3 _targetPosition;
    private Vector3 _basePosition;
    private Vector3 _vel;

    private void Awake()
    {
        _basePosition = transform.position; 
        _targetPosition = _basePosition + (Vector3)(UnityEngine.Random.insideUnitCircle * radius);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _vel, smoothSpeed);
        
        if ((transform.position - _targetPosition).sqrMagnitude < 1)
        {
            _targetPosition = _basePosition + (Vector3)(UnityEngine.Random.insideUnitCircle * radius);
        }
    }
}
