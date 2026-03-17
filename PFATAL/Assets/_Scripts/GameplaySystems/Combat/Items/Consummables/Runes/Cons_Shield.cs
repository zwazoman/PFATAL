using UnityEngine;

public class Cons_Shield : Consummable
{
    [Header("Shield Settings")]

    [SerializeField] GameObject _shieldPrefab;
    [SerializeField] float _shieldDuration = 5f;
    [SerializeField] float _shieldHp = 5f;

    public override void StartUsing()
    {
        base.StartUsing();

        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData = _shieldDuration;
        context.floatData2 = _shieldHp;

        Summoner.Instance.SpawnObject(_shieldPrefab,playerCharacter.transform.position, playerCharacter.transform.rotation,false,context);
        BreakItem();

    }
}
