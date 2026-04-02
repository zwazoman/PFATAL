using System.Threading;
using Unity.Cinemachine;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [HideInInspector] public Vector3 damagePosition;
    [HideInInspector] public DamageIndicatorManager _manager;

    [Header("References")]
    [SerializeField] Transform _indicator;

    [Header("Settings")]
    [SerializeField] float _lifeTime = 10;

    float _timer;

    private void Start()
    {
        print(damagePosition);
    }

    private void Update()
    {
        damagePosition.y = _manager.hud.playerCharacter.transform.position.y;
        Vector3 damageDirection = (damagePosition - _manager.hud.playerCharacter.transform.position).normalized;
        float angle = Vector3.SignedAngle(damageDirection, _manager.hud.playerCharacter.transform.forward,Vector3.up);

        transform.localEulerAngles = new Vector3(0, 0, angle);

        _timer += Time.deltaTime;

        if (_timer >= _lifeTime)
        {
            Destroy(gameObject);
        }
    }


}
