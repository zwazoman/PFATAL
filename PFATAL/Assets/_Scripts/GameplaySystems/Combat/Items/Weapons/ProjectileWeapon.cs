using UnityEngine;

public class ProjectileWeapon : Item
{
    [Header("References")]
    [SerializeField] public Transform shootSocket;

    [Header("Weapon Parameters")]
    [SerializeField] protected GameObject projectile;
    [SerializeField] float shootDelay;

    [SerializeField] protected LayerMask shootRayLayerMask;

    protected bool canShoot = true;
    float _timer;

    public override void StartUsing()
    {
        if (canShoot)
        {
            base.StartUsing();
        }
    }

    /// <summary>
    /// gère le delay entre 2 tirs
    /// </summary>
    async void StartShootDelay()
    {
        canShoot = false;

        while (_timer < shootDelay)
        {
            _timer += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
        _timer = 0;

        canShoot = true;
    }

    /// <summary>
    /// prend en paramètre un context, spawn le projectile donné et le tourne vers le point d'un raycast tiré depuis la caméra
    /// </summary>
    /// <param name="spawnContext"> le context du spawn</param>
    protected void Shoot(SpawnContext spawnContext)
    {
        if (shootSocket == null)
            shootSocket = main.playerCamera.transform;

        Quaternion rotation;

        RaycastHit hit;
        if (Physics.Raycast(main.playerCamera.transform.position, main.playerCamera.transform.forward, out hit, Mathf.Infinity, shootRayLayerMask))
        {
            Debug.DrawLine(main.playerCamera.transform.position, main.playerCamera.transform.position + main.playerCamera.transform.forward * 100, Color.blue, 10);
            Debug.DrawLine(shootSocket.position, hit.point, Color.red, 10);
            Vector3 direction = shootSocket.position - hit.point;
            rotation = Quaternion.LookRotation(-direction, transform.up);
        }
        else
            rotation = shootSocket.rotation;

        Summoner.Instance.SpawnObject(projectile, shootSocket.position, rotation, spawnContext);

        StartShootDelay();
    }
}
