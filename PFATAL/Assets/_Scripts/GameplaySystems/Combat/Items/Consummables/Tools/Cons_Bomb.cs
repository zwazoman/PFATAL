using UnityEngine;


public class Cons_Bomb : Consummable
{
    [SerializeField] GameObject _bombPrefab;
    [SerializeField] GameObject _explosionPrefab;

    [SerializeField] float _fuseDuration;
    float _fuseValue;

    public override void StartUsing()
    {
        base.StartUsing();

        //montre la trajectoire de la bombe
    }

    public override void UseUpdate()
    {
        base.UseUpdate();

        _fuseValue += Time.deltaTime;

        if (_fuseValue >= _fuseDuration)
            Explode();
    }

    public override void StopUsing()
    {
        base.StopUsing();

        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData = _fuseValue;
        context.floatData2 = ItemID;

        Summoner.Instance.SpawnObject(_bombPrefab, hand._itemSocket.position, hand.equippedItem.playerCharacter.playerCamera.transform.rotation, false, context);
        BreakItem();
    }

    void Explode()
    {
        //todo => explose dans tes mains - fait spawn une explosion sur le joueur
        BreakItem();
    }
}
