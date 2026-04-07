using UnityEngine;

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

        Summoner.Instance.SpawnObject(_wolfTrapPrefab, hand.visualsTransform.position, playerCharacter.transform.rotation, false);
        BreakItem();
    }
}
