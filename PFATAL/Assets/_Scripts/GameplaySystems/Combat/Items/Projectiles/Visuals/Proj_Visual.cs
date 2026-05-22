using System;
using UnityEngine;

public class Proj_Visual : MonoBehaviour
{
    public event Action OnSpawn;
    public event Action OnDespawn;

    [Header("Settings")]
    [SerializeField] protected float speed;
    [SerializeField] protected float gravity;
    [SerializeField] protected GameObject visuals;

    [HideInInspector] public SpawnContext context;
    [HideInInspector] public Projectile trueProjectile;

    protected float spawnTime;
    protected Vector3 spawnPosition;

    protected virtual async void Start()
    {
        OnSpawn?.Invoke();

        spawnTime = Time.time;
        spawnPosition = transform.position;

        if (trueProjectile == null)
        {
            while (trueProjectile == null)
                await Awaitable.NextFrameAsync();
        }

        trueProjectile.OnDespawn += () => Destroy(gameObject);
    }

    protected virtual void Update()
    {
        float timeSinceSpawn = Time.time - spawnTime;

        transform.position = spawnPosition
                     + transform.forward * (speed * timeSinceSpawn)
                     + Vector3.up * (timeSinceSpawn * timeSinceSpawn * -.5f * gravity);
    }

    protected virtual void OnDestroy()
    {
        OnDespawn?.Invoke();
    }
}
