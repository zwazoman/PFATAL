using UnityEngine;


public class Cons_Bomb : Consummable
{
    [SerializeField] GameObject _bombPrefab;

    public override void StartUsing()
    {
        base.StartUsing();

        //montre la trajectoire de la bombe
    }

    public override void StopUsing()
    {
        base.StopUsing();

        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;

        Summoner.Instance.SpawnObject(_bombPrefab, hand._itemSocket.position, hand.equippedItem.playerCharacter.playerCamera.transform.rotation, false, context);
        BreakItem();
    }
}
