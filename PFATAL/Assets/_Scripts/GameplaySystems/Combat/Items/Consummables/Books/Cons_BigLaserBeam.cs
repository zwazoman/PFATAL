using UnityEngine;

public class Cons_BigLaserBeam : Consummable
{
    [Header("Laser Settings")]
    [SerializeField] private float _maxChargeTime = 2f;

    [Header("Network")]
    [SerializeField] private GameObject _laserProjectilePrefab;

    public override void StartUsing()
    {
        if (!playerCharacter.IsOwner) return;

        playerCharacter.stateMachine.s_Frozen.Freeze(float.MaxValue);
        base.StartUsing();
    }

    public override void StopUsing()
    {
        if (!playerCharacter.IsOwner) return;
        
        playerCharacter.stateMachine.s_Frozen.Unfreeze();
        
        Transform cam = playerCharacter.playerCamera.transform;
        float chargeRatio = Mathf.Clamp01(holdDuration / _maxChargeTime);

        SpawnContext context = new SpawnContext(playerCharacter.OwnerClientId)
        {
            floatData = chargeRatio,
            floatData2 = playerCharacter.OwnerClientId
        };

        Summoner.Instance.SpawnObject(
            _laserProjectilePrefab,
            cam.position,
            Quaternion.LookRotation(cam.forward),
            false,
            context
        );

        base.StopUsing();
        BreakItem();
    }
}