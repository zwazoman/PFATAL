public class Consummable : Item
{
    protected virtual void BreakItem()
    {
        hand.DeleteItem(this);
    }
}
