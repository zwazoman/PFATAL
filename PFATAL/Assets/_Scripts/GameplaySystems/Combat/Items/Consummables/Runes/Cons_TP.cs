using UnityEngine;

public class Cons_TP : Consummable
{
    public override void StartUsing()
    {
        base.StartUsing();

        //afficher la zone de tp
    }

    public override void StopUsing()
    {
        base.StopUsing();

        //tp le joueur
        BreakItem();
    }
}
