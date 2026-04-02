using UnityEngine;
using UnityEngine.Timeline;


public class Cons_WolfTrap : Consummable
{
    [SerializeField] GameObject _wolfTrapPrefab;

    public override void StartUsing()
    {
        base.StartUsing();
    }

    public override void StopUsing()
    {
        base.StopUsing();

        Summoner.Instance.SpawnObject(_wolfTrapPrefab, carryingHand.visualsTransform.position, carryingHand.visualsTransform.rotation, false);
        BreakItem();
    }
}
