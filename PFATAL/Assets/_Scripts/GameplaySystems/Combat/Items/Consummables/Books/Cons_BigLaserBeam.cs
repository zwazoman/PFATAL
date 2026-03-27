using Unity.Netcode;
using UnityEngine;

public class Cons_BigLaserBeam : Consummable
{
    [Header("Laser Settings")]
    [SerializeField] private float _maxChargeTime = 2f;

    [Header("Network")]
    [SerializeField] private GameObject _laserProjectilePrefab;

    public override void StopUsing()
    {
        Transform cam = playerCharacter.playerCamera.transform;
        float chargeRatio = Mathf.Clamp01(holdDuration / _maxChargeTime);

        FireLaserRpc(cam.position, cam.forward, chargeRatio);

        base.StopUsing();
        BreakItem();
    }

    [Rpc(SendTo.Server)]
    void FireLaserRpc(Vector3 origin, Vector3 direction, float chargeRatio)
    {
        GameObject obj = Instantiate(_laserProjectilePrefab, origin, Quaternion.LookRotation(direction));
        NetworkObject netObj = obj.GetComponent<NetworkObject>();
        netObj.Spawn();

        if (obj.TryGetComponent(out Proj_BigLaserBeam laser))
        {
            laser.Fire(origin, direction, chargeRatio, playerCharacter.OwnerClientId);
        }
    }
}