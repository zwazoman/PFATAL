using System;
using UnityEngine;

public class Cons_Tornado : Cons_BookBase
{
    [Header("Network")]
    [SerializeField] private GameObject _tornadoProjectilePrefab;

    protected override void ApplySpellEffect()
    {
        print("spell");

        //spawn tornado
        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;

        base.SpawnSpell(context, Quaternion.identity, playerCharacter.transform.position);

        /*_ = Summoner.Instance.SpawnObject(_tornadoProjectilePrefab,
            playerCharacter.transform.position + playerCharacter.transform.forward,
            playerCharacter.transform.rotation, true, context);*/
    }
}
