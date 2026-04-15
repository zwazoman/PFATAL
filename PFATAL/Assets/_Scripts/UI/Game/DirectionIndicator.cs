using _scripts.PlayerCharacter;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class DirectionIndicator : MonoBehaviour
{
    Vector3 _targetPosition;
    PlayerCharacter _playerCharacter;

    [Header("Scene References")]
    [SerializeField] Image _image;

    [Header("Settings")]
    [SerializeField] bool _isScreenspace = false;
    [SerializeField] bool _useLifetime = true;
    [SerializeField] float _lifeTime = 10;

    [Header("Screenspace Settings")]
    [SerializeField] float _smooth = .1f;

    float _timer;
    float _currentVelocity;

    PooledObject _thisPooledObject;
    
    void OnInstantiatedByPool()
    {
        TryGetComponent(out _thisPooledObject);
    }

    void OnPulledFromPool()
    {
        DOTween.Kill(this);
        _image.color = _image.color.WithAlpha(1);
        DOTween.To(
            () => _image.color.a,
            x => _image.color = _image.color.WithAlpha(x),
            0, _lifeTime).SetEase(Ease.InCubic).SetTarget(this);
    }
    
    
    public void Setup(Vector3 targetPosition, PlayerCharacter playerCharacter)
    {
        _targetPosition = targetPosition;
        _playerCharacter = playerCharacter;
    }

    public void SetTargetpos(Vector3 targetPos)
    {
        _targetPosition = targetPos;
    }

    private void Update()
    {
        float angle = 0;
        if( _isScreenspace)
        {
            Vector3 playerToTarget = _playerCharacter.playerCamera.transform.position - _targetPosition;
            float dot = Vector3.Dot(_playerCharacter.playerCamera.transform.forward, playerToTarget);

            Vector2 targetScreenPos = new Vector2(_playerCharacter.playerCamera.WorldToScreenPoint(_targetPosition).x, _playerCharacter.playerCamera.WorldToScreenPoint(_targetPosition).y);
            Vector2 screenCenterPos = new Vector2(Screen.width / 2, Screen.height / 2);

            float targetAngle = Vector2.SignedAngle(Vector2.up, (targetScreenPos - screenCenterPos).normalized);

            if (dot > 0)
            {
                targetAngle = -targetAngle;
            }

            angle = Mathf.SmoothDampAngle(transform.localEulerAngles.z, targetAngle, ref _currentVelocity, _smooth);
        }
        else
        {
            _targetPosition.y = _playerCharacter.transform.position.y;
            Vector3 damageDirection = (_targetPosition - _playerCharacter.transform.position).normalized;
            angle = Vector3.SignedAngle(damageDirection, _playerCharacter.transform.forward, Vector3.up);
        }

        transform.localEulerAngles = new Vector3(0, 0, angle);

        if (_useLifetime)
        {
            _timer += Time.deltaTime;

            if (_timer >= _lifeTime)
            {
                _thisPooledObject.GoBackIntoPool();
            }
        }
    }
}
