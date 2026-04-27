public class Visual_Proj_Crossbow : Proj_Visual
{
    protected override void Start()
    {
        speed *= 1 + context.floatData * 2;

        base.Start();
    }
}
