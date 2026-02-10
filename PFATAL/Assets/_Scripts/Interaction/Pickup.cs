using Unity.Netcode;
using UnityEngine;

public class Pickup : Interactable
{
    [SerializeField] ItemScriptable _item;

    PlayerInteraction _currentPlayerInteraction;
    //Item _currentItem = null;

    public override void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        _currentPlayerInteraction = interaction;

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
