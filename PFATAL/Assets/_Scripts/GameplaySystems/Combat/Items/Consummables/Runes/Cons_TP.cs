using UnityEngine;

[CreateAssetMenu(fileName = "Tp", menuName = "Item/Consummables/Runes/Tp")]
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
