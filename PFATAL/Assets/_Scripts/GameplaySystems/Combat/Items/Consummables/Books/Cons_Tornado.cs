using System;
using UnityEngine;

public class Cons_Tornado : Cons_BookBase
{
    public override void StopUsing()
    {
        base.StopUsing();
        StartCastAnimation();
    }

    protected override void ApplySpellEffect()
    {
        //spawn tornado
        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;

        base.SpawnSpell(context, Quaternion.identity, playerCharacter.transform.position);

        /*_ = Summoner.Instance.SpawnObject(_tornadoProjectilePrefab,
            playerCharacter.transform.position + playerCharacter.transform.forward,
            playerCharacter.transform.rotation, true, context);*/
    }
}
