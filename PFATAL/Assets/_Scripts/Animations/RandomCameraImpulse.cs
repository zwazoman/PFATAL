using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class RandomCameraImpulse : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CinemachineImpulseSource _impulseSource;

    [Header("Settings")]
    [SerializeField] float _minTimeBetweenShakes = 3;
    [SerializeField] float _maxTimeBetweenShakes = 10;
    [SerializeField] Transform[] _explosionSoundPos;

    float _timeBetweenShakes = 0;
    float _timer = 0;

    private void Start()
    {
        _timeBetweenShakes = Random.Range(_minTimeBetweenShakes, _maxTimeBetweenShakes);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if(_timer >= _timeBetweenShakes)
        {
            _timeBetweenShakes = Random.Range(_minTimeBetweenShakes, _maxTimeBetweenShakes);
            _timer = 0;
            _impulseSource.GenerateImpulse();
            if (AudioManager.Instance.playSounds)
                AudioManager.Instance.PlayOneShot(Sounds.Explosion3D, _explosionSoundPos[Random.Range(0, _explosionSoundPos.Length)].position);
        }
    }
}
