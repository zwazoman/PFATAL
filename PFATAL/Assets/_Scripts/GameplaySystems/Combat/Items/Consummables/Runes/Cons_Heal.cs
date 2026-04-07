using UnityEngine;

public class Cons_Heal : Consummable
{
    [SerializeField] private float _heal = 5f;

    public override void StartUsing()
    {
        base.StartUsing();

        if (playerCharacter.TryGetComponent(out DamageableObject health))
        {
            health.Heal(_heal);
        }

        BreakItem();
    }
}