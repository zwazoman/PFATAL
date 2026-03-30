using UnityEngine;

public class Cons_GroundSlam : Consummable
{
    [Header("Ground Slam Settings")]
    [SerializeField] float _dashStrength = 50;

    public override void StartUsing()
    {
        base.StartUsing();

        playerCharacter.physics.AddImpulse(Vector3.down);

        //todo => ajouter un component au joueur qui créera l'explosion au prochain contact avec le sol et se détruiera ensuite
        //ou juste gérer la logique sur celui là et ne détruire que le visuel ? en mode animation break -> attendre l'explosion -> swap a l'item d'après

        BreakItem();
    }
}
