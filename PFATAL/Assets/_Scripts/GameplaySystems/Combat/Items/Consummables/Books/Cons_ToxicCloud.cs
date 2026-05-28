using UnityEngine;

public class Cons_ToxicCloud : Cons_BookBase
{    
    protected override void ApplySpellEffect()
    {
        //spawn poison projectile
        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;

        base.SpawnSpell(context, Quaternion.identity, playerCharacter.playerCamera.transform.position);

        /*_ = Summoner.Instance.SpawnObject(
            _projToxicCloudPrefab,
            hand._itemSocket.position,
            playerCharacter.cameraBehaviour.transform.rotation,
            false, context);*/
    }
}