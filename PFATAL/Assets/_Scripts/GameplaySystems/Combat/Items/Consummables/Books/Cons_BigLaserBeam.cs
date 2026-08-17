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
        if(!_spellAnimationIsPlaying)
            playerCharacter.stateMachine.s_Frozen.Freeze(float.MaxValue);

        base.StartUsing();
    }

    public override void UnEquip()
    {
        base.UnEquip();
        playerCharacter.stateMachine.s_Frozen.Unfreeze();
    }

    protected override void ApplySpellEffect()
    {
        playerCharacter.stateMachine.s_Frozen.Unfreeze();

        Transform cam = playerCharacter.playerCamera.transform;
        float chargeRatio = Mathf.Clamp01(holdDuration / _maxChargeTime);

        // Spawn the laser projectile
        SpawnContext context = new SpawnContext(playerCharacter.OwnerClientId)
        {
            floatData = chargeRatio,
            floatData2 = ItemID
        };

        _=Summoner.Instance.SpawnObject(
            _laserProjectilePrefab,
            cam.position,
            Quaternion.LookRotation(cam.forward),
            false,
            context
        );

        // Apply recoil to the player
        playerCharacter.physics.SetVelocity(-playerCharacter.cameraBehaviour.transform.forward * _recoilForce);

    }
}