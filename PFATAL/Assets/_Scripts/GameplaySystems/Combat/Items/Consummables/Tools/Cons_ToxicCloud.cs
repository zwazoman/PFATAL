using UnityEngine;

public class Cons_ToxicCloud : Consummable
{
    [SerializeField] private GameObject _projToxicCloudPrefab;

    public override void StopUsing()
    {
        base.StopUsing();

        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;

        Summoner.Instance.SpawnObject(_projToxicCloudPrefab, hand._itemSocket.position, playerCharacter.cameraBehaviour.transform.rotation, false, context);

        BreakItem();
    }
}