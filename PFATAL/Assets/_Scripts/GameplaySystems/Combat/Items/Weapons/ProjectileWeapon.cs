using UnityEngine;

public class ProjectileWeapon : Item
{
    [Header("References")]
    [SerializeField] public Transform shootSocket;

    [Header("Weapon Settings")]
    [SerializeField] protected GameObject projectile;
    [SerializeField] float shootDelay;

    [SerializeField] protected LayerMask shootRayLayerMask;

    protected bool canShoot = true;
    float _timer;

    /// <summary>
    /// g�re le delay entre 2 tirs
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
    /// prend en param�tre un context, spawn le projectile donn� et le tourne vers le point d'un raycast tir� depuis la cam�ra
    /// </summary>
    /// <param name="spawnContext"> le context du spawn</param>
    protected async Awaitable<GameObject> Shoot(SpawnContext spawnContext)
    {
        if (shootSocket == null)
            shootSocket = playerCharacter.playerCamera.transform;

        Quaternion rotation;

        RaycastHit hit;
        if (Physics.Raycast(playerCharacter.playerCamera.transform.position, playerCharacter.playerCamera.transform.forward, out hit, Mathf.Infinity, shootRayLayerMask))
        {
            Debug.DrawLine(playerCharacter.playerCamera.transform.position, playerCharacter.playerCamera.transform.position + playerCharacter.playerCamera.transform.forward * 100, Color.blue, 10);
            Debug.DrawLine(shootSocket.position, hit.point, Color.red, 10);
            Vector3 direction = shootSocket.position - hit.point;
            rotation = Quaternion.LookRotation(-direction, transform.up);
        }
        else
            rotation = shootSocket.rotation;

        StartShootDelay();

        return await Summoner.Instance.SpawnObject(projectile, shootSocket.position, rotation,true, spawnContext);
    }
}
