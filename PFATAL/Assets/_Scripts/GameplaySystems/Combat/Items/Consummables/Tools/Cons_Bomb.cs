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

        Summoner.Instance.SpawnObject(_bombPrefab, carryingHand.visualsTransform.position, carryingHand.visualsTransform.rotation, false);
        BreakItem();
    }
}
