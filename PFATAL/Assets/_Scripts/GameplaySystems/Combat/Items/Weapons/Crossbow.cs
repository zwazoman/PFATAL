using _scripts.PlayerCharacter;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
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

    [SerializeField] float _maxChargeTime = 1.5f;

    [SerializeField] float _chargeZoomThreshold = .3f;

    [SerializeField] LayerMask _layerMask;

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

        chargevalue += Time.deltaTime / _maxChargeTime;

        if(chargevalue >= 1)
        {
            isCharged = true;
            chargevalue = _maxChargeTime;
            print("crossbow fully charged");
        }
        else if(chargevalue >= _chargeZoomThreshold)
        {
            //début zoom caméra
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
        spawnContext.floatData = chargeValue;

        if (_shootSocket == null)
            _shootSocket = main.playerCamera.transform;

        Quaternion rotation;

        RaycastHit hit;
        if(Physics.Raycast(main.playerCamera.transform.position, main.playerCamera.transform.forward, out hit, Mathf.Infinity, _layerMask))
        {
            print(hit.collider.gameObject.name);
            Debug.DrawLine(main.playerCamera.transform.position, main.playerCamera.transform.position + main.playerCamera.transform.forward * 100, Color.blue, 10);
            Debug.DrawLine(_shootSocket.position, hit.point, Color.red, 10);
            Vector3 direction = _shootSocket.position - hit.point;
            rotation = Quaternion.LookRotation(-direction, transform.up);
        }
        else
            rotation = _shootSocket.rotation;

        Summoner.Instance.SpawnObject(_projectile, _shootSocket.position, rotation, spawnContext);

        StartShootDelay();
    }

    void StopCameraZoom()
    {

    }
}
