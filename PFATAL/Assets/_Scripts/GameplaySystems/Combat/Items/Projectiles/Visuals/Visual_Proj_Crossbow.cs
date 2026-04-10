using UnityEngine;

public class Visual_Proj_Crossbow : Proj_Visual
{
    protected override void Start()
    {
        print("allo");
        speed *= 1 + context.floatData * 2;

        base.Start();
    }
}
