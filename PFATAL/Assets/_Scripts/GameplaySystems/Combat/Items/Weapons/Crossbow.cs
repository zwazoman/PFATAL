using _scripts.PlayerCharacter;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;

public class Crossbow : Item
{
    [Header("References")]
    [SerializeField] public Transform _shootSocket;

    [Header("Weapon Parameters")]
    [SerializeField] GameObject _projectile;
    [SerializeField] float tmpDelay;
    [SerializeField] float rateOfFire;

    protected bool canShoot = true;
    protected bool isWaiting = false;

    float _timer = 0;

    //todo context dans le shoot 

    public override void OnPickup(PlayerCharacter main, Hand hand)
    {
        base.OnPickup(main, hand);
        canShoot = true;
        isWaiting = false;
    }

    public override void StartUsing()
    {
        base.StartUsing();
        TryShoot();
    }

    public override void UseUpdate()
    {
        TryShoot();
    }

    public async void ShootDelay()
    {
        if (isWaiting)
            return;

        isWaiting = true;

        while (_timer < tmpDelay)
        {
            _timer += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
        _timer = 0;

        canShoot = true;
        isWaiting = false;
    }

    public override void StopUsing()
    {
        base.StopUsing();
    }

    public virtual bool TryShoot()
    {
        if (canShoot)
        {
            Shoot();
            canShoot = false;
            ShootDelay();
            return true;
        }
        return false;
    }

    async void Shoot()
    {
        SpawnContext spawnContext = new(NetworkManager.Singleton.LocalClientId);

        if (_shootSocket == null)
        {
            _shootSocket = main.playerCamera.transform;
        }

        GameObject projectile = await Summoner.Instance.SpawnObject(_projectile, _shootSocket.position, _shootSocket.rotation, spawnContext);
        Debug.Log(projectile.name);
    }
}
