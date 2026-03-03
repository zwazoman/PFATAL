using Unity.Netcode;
using UnityEngine;

public class Pickup : Interactable
{
    [SerializeField] Item _item;

    public override void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        print("pickup par pitié");
        print(interaction);

        if (interaction.main.playerHands.TryEquipItem(_item))
            DespawnRpc();
        else
            print("couldn't equip item");

    }

    [Rpc(SendTo.Server)]
    void DespawnRpc()
    {
        NetworkObject.Despawn();
    }
}
