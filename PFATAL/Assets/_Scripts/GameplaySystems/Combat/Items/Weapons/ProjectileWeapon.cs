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
    protected void Shoot(SpawnContext spawnContext)
    {
        if (shootSocket == null)
            shootSocket = _playerCharacter.playerCamera.transform;

        Quaternion rotation;

        RaycastHit hit;
        if (Physics.Raycast(_playerCharacter.playerCamera.transform.position, _playerCharacter.playerCamera.transform.forward, out hit, Mathf.Infinity, shootRayLayerMask))
        {
            Debug.DrawLine(_playerCharacter.playerCamera.transform.position, _playerCharacter.playerCamera.transform.position + _playerCharacter.playerCamera.transform.forward * 100, Color.blue, 10);
            Debug.DrawLine(shootSocket.position, hit.point, Color.red, 10);
            Vector3 direction = shootSocket.position - hit.point;
            rotation = Quaternion.LookRotation(-direction, transform.up);
        }
        else
            rotation = shootSocket.rotation;

        Summoner.Instance.SpawnObject(projectile, shootSocket.position, rotation,false, spawnContext);

        StartShootDelay();
    }
}
