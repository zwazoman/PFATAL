using System.Threading;
using Unity.Cinemachine;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    Vector3 _targetPosition;
    Transform _playerTransform;

    [Header("Settings")]
    [SerializeField] bool _useLifetime = true;
    [SerializeField] float _lifeTime = 10;

    float _timer;

    public void Setup(Vector3 targetPosition, Transform playerTransform)
    {
        _targetPosition = targetPosition;
        _playerTransform = playerTransform;
    }

    public void SetTargetpos(Vector3 targetPos)
    {
        _targetPosition = targetPos;
    }

    private void Update()
    {
        _targetPosition.y = _playerTransform.position.y;
        Vector3 damageDirection = (_targetPosition - _playerTransform.position).normalized;
        float angle = Vector3.SignedAngle(damageDirection, _playerTransform.forward,Vector3.up);

        transform.localEulerAngles = new Vector3(0, 0, angle);

        if (_useLifetime)
        {
            _timer += Time.deltaTime;

            if (_timer >= _lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
