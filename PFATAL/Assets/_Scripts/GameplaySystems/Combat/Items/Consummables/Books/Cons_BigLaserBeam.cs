using UnityEngine;

public class Cons_BigLaserBeam : Cons_BookBase
{
    [Header("Laser Settings")]
    [SerializeField] private float _maxChargeTime = 2f;
    [SerializeField] private float _recoilForce = 2f;

    [Header("Network")]
    [SerializeField] private GameObject _laserProjectilePrefab;

    public override void StartUsing()
    {
        if (used) return;

        playerCharacter.stateMachine.s_Frozen.Freeze(float.MaxValue);
        base.StartUsing();
    }

    protected override void ApplySpellEffect()
    {
        playerCharacter.stateMachine.s_Frozen.Unfreeze();
        playerCharacter.physics.SetVelocity(Vector3.zero);

        Transform cam = playerCharacter.playerCamera.transform;
        float chargeRatio = Mathf.Clamp01(holdDuration / _maxChargeTime);

        SpawnContext context = new SpawnContext(playerCharacter.OwnerClientId)
        {
            floatData = chargeRatio,
            floatData2 = ItemID
        };

        Summoner.Instance.SpawnObject(
            _laserProjectilePrefab,
            cam.position,
            Quaternion.LookRotation(cam.forward),
            false,
            context
        );

        playerCharacter.physics.SetVelocity(Vector3.zero);
        playerCharacter.physics.AddImpulse(-playerCharacter.cameraBehaviour.transform.forward * _recoilForce);

        //BreakItem();
    }
}