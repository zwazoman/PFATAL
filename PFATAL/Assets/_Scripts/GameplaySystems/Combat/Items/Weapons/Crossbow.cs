using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "Crossbow", menuName = "Item/Weapon/Crossbow")]
public class Crossbow : ItemScriptable
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
            Debug.Log("shoot");
            ShootRpc();
            canShoot = false;
            ShootDelay();
            return true;
        }
        return false;
    }

    async void ShootRpc()
    {
        SpawnContext spawnContext = new(NetworkManager.Singleton.LocalClientId);

        if (_shootSocket != null)
        {
            GameObject projectile =  await Summoner.Instance.SpawnObject("crossbowProj", _shootSocket.position, _shootSocket.rotation, spawnContext);
            Debug.Log(projectile.name);
        }
        else
        {
            GameObject projectile = await Summoner.Instance.SpawnObject("crossbowProj", main.playerCamera.transform.position, main.playerCamera.transform.rotation, spawnContext);
            Debug.Log(projectile.name);
        }
    }
}
