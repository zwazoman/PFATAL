using NetworkTime;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Proj_Visual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _speed;
    [SerializeField] float _gravity;
    [SerializeField] float _lifeTime = 3f;

    [HideInInspector] public SpawnContext context;

    float _spawnTime;
    Vector3 _spawnPosition;

    private void Start()
    {
        print("con");

        _spawnTime = Time.time;
        _spawnPosition = transform.position;

        _speed *= 1 + context.floatData * 2;
    }

    private void Update()
    {
        float timeSinceSpawn = Time.time - _spawnTime;

        transform.position = _spawnPosition
                     + transform.forward * (_speed * timeSinceSpawn)
                     + Vector3.up * (timeSinceSpawn * timeSinceSpawn * -.5f * _gravity);

        if(timeSinceSpawn > _lifeTime)
            Destroy(gameObject);
    }
}
