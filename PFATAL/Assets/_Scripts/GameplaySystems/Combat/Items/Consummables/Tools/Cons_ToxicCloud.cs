using UnityEngine;

public class Cons_ToxicCloud : Consummable
{
    [SerializeField] private GameObject _projToxicCloudPrefab;

    public override void StopUsing()
    {
        base.StopUsing();

        Summoner.Instance.SpawnObject(_projToxicCloudPrefab, hand._itemSocket.position, playerCharacter.cameraBehaviour.transform.rotation, false);

        BreakItem();
    }
}