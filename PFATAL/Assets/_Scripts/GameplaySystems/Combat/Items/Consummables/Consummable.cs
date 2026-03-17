using UnityEngine;

public class Consummable : Item
{
    protected virtual void BreakItem()
    {
        carryingHand.DeleteItem(this);
    }
}
