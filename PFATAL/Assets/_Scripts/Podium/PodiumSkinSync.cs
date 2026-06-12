using Unity.Netcode;
using UnityEngine;

public class PodiumSkinSync : NetworkBehaviour
{
    [SerializeField] PodiumUI _podiumUI;

    public override void OnNetworkSpawn()
    {
        // Chaque client envoie son skin au serveur
        SendSkinServerRpc(NetworkManager.LocalClientId, PlayerPrefs.GetInt("skinID"));
    }

    [Rpc(SendTo.Server)]
    void SendSkinServerRpc(ulong clientId, int skinID)
    {
        // Le serveur broadcast à tout le monde
        ReceiveSkinRpc(clientId, skinID);
    }

    [Rpc(SendTo.Everyone)]
    void ReceiveSkinRpc(ulong clientId, int skinID)
    {
        _podiumUI.ApplySkin(clientId, skinID);
    }
}