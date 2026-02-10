using Unity.Netcode;
using UnityEngine;

public class Pickup : Interactable
{
    [SerializeField] ItemScriptable _item;

    PlayerInteraction _currentPlayerInteraction;
    //Item _currentItem = null;

    public async override void Interact(PlayerInteraction interaction)
    {
        base.Interact(interaction);

        _currentPlayerInteraction = interaction;

        if (interaction.main.playerHands.TryEquipItem(_item))
            DespawnRpc();
        else
            print("couldn't equip item");


        //SpawnNewItemRPC();

        //while (_currentItem == null)
        //{
        //    await Awaitable.NextFrameAsync();
        //}

        //_currentPlayerInteraction.main.playerHands.Equip(_currentItem);
    }

    [Rpc(SendTo.Server)]
    void DespawnRpc()
    {
        NetworkObject.Despawn();
    }

    //[Rpc(SendTo.Server)]
    //void SpawnNewItemRPC(RpcParams rpcParams = default)
    //{
    //    NetworkObject itemNetwork = NetworkObject.InstantiateAndSpawn(_itemPrefab, NetworkManager, rpcParams.Receive.SenderClientId, true, true, false, transform.position, transform.rotation);

    //    ReceiveItemRPC(itemNetwork.GetNetworkBehaviourAtOrderIndex(0), RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));

    //    NetworkObject.Despawn();
    //}

    //[Rpc(SendTo.SpecifiedInParams)]
    //void ReceiveItemRPC(NetworkBehaviourReference itemNetworkRef, RpcParams rpcParams)
    //{
    //    print(rpcParams.Receive.SenderClientId);
    //    print(itemNetworkRef.TryGet(out Weapon weapon));

    //    if(itemNetworkRef.TryGet(out Item item))
    //        print(item.gameObject.name);

    //    //_currentPlayerInteraction.main.playerHands.Equip(item);
    //    _currentItem = item;
    //}

}
