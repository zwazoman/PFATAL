using UnityEngine;

public class FlowingRiver : MonoBehaviour
{
    [SerializeField] Transform _riverStart;
    [SerializeField] Transform _riverEnd;

    Vector3 _riverDirection;
    float _riverLength;
    

    private void Start()
    {
        _riverDirection = (_riverEnd.position - _riverStart.position).normalized;  
        _riverLength = Vector3.Distance(_riverStart.position,_riverEnd.position);
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.localPlayerCharacter == null)
            return;

        Vector3 toPlayer = GameManager.Instance.localPlayerCharacter.transform.position - _riverStart.position;

        float distanceAlongRiver = Vector3.Dot(toPlayer, _riverDirection);

        distanceAlongRiver = Mathf.Clamp(distanceAlongRiver, 0f, _riverLength);

        transform.position = _riverStart.position + _riverDirection * distanceAlongRiver;

        Debug.DrawLine(_riverStart.position, _riverEnd.position, Color.red);
        Debug.DrawLine(
            GameManager.Instance.localPlayerCharacter.transform.position,
            transform.position,
            Color.green);

    }
}
