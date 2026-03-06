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

    [SerializeField] float _maxCharge = 1;
    [SerializeField] float _chargeMultiplyer = 1;

    bool startedShooting = false;
    bool canShoot = true;
    bool isCharged = false;
    float chargevalue;

    float _timer = 0;

    //todo context dans le shoot 

    public override void OnPickup(PlayerCharacter main, Hand hand)
    {
        base.OnPickup(main, hand);
        canShoot = true;
    }

    public override void StartUsing()
    {
        if (canShoot)
        {
            startedShooting = true;
            base.StartUsing();
        }
    }

    public override void UseUpdate()
    {
        if (isCharged)
            return;

        chargevalue += Time.deltaTime * _chargeMultiplyer;

        if(chargevalue >= _maxCharge)
        {
            isCharged = true;
            chargevalue = _maxCharge;
            print("crossbow fully charged");
        }
    }

    public override void StopUsing()
    {
        base.StopUsing();

        if (!canShoot || !startedShooting)
            return;

        Shoot(chargevalue);
        chargevalue = 0;
        isCharged = false;
        startedShooting = false;
    }

    public async void StartShootDelay()
    {
        canShoot = false;

        while (_timer < tmpDelay)
        {
            _timer += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
        _timer = 0;

        canShoot = true;
    }


    void Shoot(float chargeValue)
    {
        SpawnContext spawnContext = new(NetworkManager.Singleton.LocalClientId);
        spawnContext.data = chargeValue;

        if (_shootSocket == null)
            _shootSocket = main.playerCamera.transform;

        Summoner.Instance.SpawnObject(_projectile, _shootSocket.position, _shootSocket.rotation, spawnContext);

        StartShootDelay();
    }
}
