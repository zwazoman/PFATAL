using NetworkTime;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Proj_Visual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected float speed;
    [SerializeField] protected float gravity;
    [SerializeField] protected GameObject visuals;

    [HideInInspector] public SpawnContext context;
    [HideInInspector] public Projectile _mirrorProjectile;

    float _spawnTime;
    Vector3 _spawnPosition;

    protected virtual void Start()
    {
        print("con");

        _spawnTime = Time.time;
        _spawnPosition = transform.position;

        _mirrorProjectile.OnDespawn += ()=> Destroy(gameObject);
    }

    protected virtual void Update()
    {
        float timeSinceSpawn = Time.time - _spawnTime;

        transform.position = _spawnPosition
                     + transform.forward * (speed * timeSinceSpawn)
                     + Vector3.up * (timeSinceSpawn * timeSinceSpawn * -.5f * gravity);
    }
}
