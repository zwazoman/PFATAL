using UnityEngine;

public class Cons_ToxicCloud : Cons_BookBase
{
    [SerializeField] private GameObject _projToxicCloudPrefab;
    
    public override void StopUsing()
    {
        base.StopUsing();
        StartCastAnimation();
    }

    protected override void ApplySpellEffect()
    {
        //spawn poison projectile
        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;
        _ = Summoner.Instance.SpawnObject(
            _projToxicCloudPrefab,
            hand._itemSocket.position,
            playerCharacter.cameraBehaviour.transform.rotation,
            false, context);
    }
}