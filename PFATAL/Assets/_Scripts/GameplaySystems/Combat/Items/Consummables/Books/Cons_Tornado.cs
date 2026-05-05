using UnityEngine;

public class Cons_Tornado : Consummable
{
    [Header("Network")]
    [SerializeField] private GameObject _tornadoProjectilePrefab;

    public override void StartUsing()
    {
        base.StartUsing();
        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;

        _ = Summoner.Instance.SpawnObject(_tornadoProjectilePrefab,
            playerCharacter.transform.position + playerCharacter.transform.forward,
            playerCharacter.transform.rotation, true, context);
        
        BreakItem();
    }
}
