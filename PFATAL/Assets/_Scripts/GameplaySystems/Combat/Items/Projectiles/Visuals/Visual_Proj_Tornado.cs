using UnityEngine;

public class Visual_Proj_Tornado : Proj_Visual
{
    [SerializeField] float _duringTime = 5f;
    private Vector3 _tornadoSpawnPosition;
    private float _tornadoSpawnTime;

    protected override async void Start()
    {
        base.Start();

        while (trueProjectile == null)
            await Awaitable.NextFrameAsync();

        spawnPosition = trueProjectile.transform.position;
        spawnTime = Time.time;
    }

    protected override void Update()
    {
        float elapsed = Time.time - spawnTime;
        float halfTime = _duringTime / 2f;
        float distanceTravelled;

        if (elapsed < halfTime)
            distanceTravelled = speed * elapsed;
        else
            distanceTravelled = speed * halfTime + (speed / 2f) * (elapsed - halfTime);

        transform.position = spawnPosition + transform.forward * distanceTravelled;
    }
}
