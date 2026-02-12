using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "newItem", menuName = "Item/Weapon")]
public class Weapon : ItemScriptable
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

    [Rpc(SendTo.Server)]
    void ShootRpc()
    {
        if(_shootSocket != null)
            NetworkObject.InstantiateAndSpawn(_projectile, NetworkManager.Singleton, 0, true, true, false, _shootSocket.position, _shootSocket.rotation);
        else
            NetworkObject.InstantiateAndSpawn(_projectile, NetworkManager.Singleton, 0, true, true, false, main.transform.position, main.transform.rotation);
    }
}
