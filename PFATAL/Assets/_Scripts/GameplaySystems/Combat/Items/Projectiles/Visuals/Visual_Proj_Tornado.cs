using UnityEngine;

public class Visual_Proj_Tornado : Proj_Visual
{
    [SerializeField] float _duringTime = 5f;

    private Vector3 _spawnPosition;
    private float _spawnTime;

    protected override void Start()
    {
        _spawnPosition = transform.position;
        _spawnTime = Time.time;

        StartCoroutine(WaitForProjectile());
    }

    System.Collections.IEnumerator WaitForProjectile()
    {
        while (trueProjectile == null)
            yield return null;

        trueProjectile.OnDespawn += () => Destroy(gameObject);
    }

    protected override void Update()
    {
        float elapsed = Time.time - _spawnTime;
        float halfTime = _duringTime / 2f;
        float distanceTravelled;

        if (elapsed < halfTime)
            distanceTravelled = speed * elapsed;
        else
            distanceTravelled = speed * halfTime + (speed / 2f) * (elapsed - halfTime);

        transform.position = _spawnPosition + transform.forward * distanceTravelled;
    }
}