using UnityEngine;

public class Cons_Tornado : Cons_BookBase
{
    [Header("Network")]
    [SerializeField] private GameObject _tornadoProjectilePrefab;

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

        _ = Summoner.Instance.SpawnObject(_tornadoProjectilePrefab,
            playerCharacter.transform.position + playerCharacter.transform.forward,
            playerCharacter.transform.rotation, true, context);
    }
}
