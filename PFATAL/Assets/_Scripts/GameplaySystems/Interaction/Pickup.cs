using Unity.Netcode;
using UnityEngine;

public class Pickup : Interactable
{
    [SerializeField] Item _item;

    [SerializeField] GameObject _itemPrefab;

    public override void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        Item item;

        if(_itemPrefab.TryGetComponent(out item))
        {
            if (interaction.main.playerHands.TryEquipItem(_item, _itemPrefab))
                DespawnRpc();
            else
                print("couldn't equip item");
        }
        else
        {
            print("no item on item prefab");
        }

        //if (interaction.main.playerHands.TryEquipItem(_item))
        //    DespawnRpc();
        //else
        //    print("couldn't equip item");

    }

    [Rpc(SendTo.Server)]
    void DespawnRpc()
    {
        NetworkObject.Despawn();
    }
}
