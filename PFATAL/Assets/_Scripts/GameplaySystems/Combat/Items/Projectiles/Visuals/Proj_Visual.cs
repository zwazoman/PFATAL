using UnityEngine;

public class Proj_Visual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected float speed;
    [SerializeField] protected float gravity;
    [SerializeField] protected GameObject visuals;

    [HideInInspector] public SpawnContext context;
    [HideInInspector] public Projectile trueProjectile;

    protected float spawnTime;
    Vector3 _spawnPosition;

    protected virtual async void Start()
    {
        spawnTime = Time.time;
        _spawnPosition = transform.position;

        while (trueProjectile == null)
        {
            await Awaitable.NextFrameAsync();
        }
        trueProjectile.OnDespawn += () => Destroy(gameObject);
    }

    protected virtual void Update()
    {
        float timeSinceSpawn = Time.time - spawnTime;

        transform.position = _spawnPosition
                     + transform.forward * (speed * timeSinceSpawn)
                     + Vector3.up * (timeSinceSpawn * timeSinceSpawn * -.5f * gravity);
    }
}
