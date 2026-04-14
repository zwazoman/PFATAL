using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;



public class SteamPlayerList : NetworkBehaviour
{
    public static SteamPlayerList Instance;

    public List<PermanentPlayerIdentity> Players = new();
    

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
    }

    public override void OnNetworkSpawn()
    {
        if (Players == null)
        {
            Debug.LogError("Players est NULL !");
            return;
        }
        
        Debug.Log("[SteamPlayerList] NetworkSpawn OK");
        
        if(NetworkManager.Singleton.IsServer){
            Debug.Log("[SteamPlayerList] NetworkSpawn JE PASSE PAR LA ");
            NetworkManager.Singleton.OnClientConnectedCallback += OnNewPlayerJoined;
        }
    }

    private void OnNewPlayerJoined(ulong newClientNetworkID)
    {
        Debug.Log("[SteamPlayerList] NewPlayerJoined ");
        Debug.Log("[SteamPlayerList] NewPlayerJoined " + newClientNetworkID);
        SendPlayerListToNewClientRPC(
            Players.ToArray(),
            RpcTarget.Single(newClientNetworkID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendPlayerListToNewClientRPC(PermanentPlayerIdentity[] players, RpcParams rpcParams = default)
    {
        Players = players.ToList();
        UpdateGameManagerDictionnary();
    }


    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Everyone)]
    public void AddPlayerRpc(ulong NetworkClientId, string steamId, string playerName, string tempUnityId,RpcParams rpcParams = default)
    {
        //met à jour la liste de joueurs chez tout le monde
        PermanentPlayerIdentity data = new (
            playerName,
            steamId,
            PermanentPlayerIdentity.ePlatform.Steam,
            NetworkClientId,
            tempUnityId);
        Players.Add(data);
        
        //met à jour le dico du gamemanager
        UpdateGameManagerDictionnary();

        //debug
        foreach (var p in Players)
            Debug.LogWarning($"[SteamPlayerList] Joueur déjà présent : {playerName}");
        Debug.Log($"[SteamPlayerList] Ajout : {playerName}");
        
        if (!NetworkManager.Singleton.IsServer) return;
        
        //=== code server ==
        
        // Notifie GameLobby avec le clientId du sender pour mettre à jour le nom Steam
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"[SteamPlayerList] senderClientId: {senderClientId} + {playerName}");
        GameLobby.Instance?.OnSteamPlayerRegistered(senderClientId, playerName, steamId);
    }

    private void UpdateGameManagerDictionnary()
    {
        if (Players.Count == 0) return;
    
        Dictionary<ulong, PermanentPlayerIdentity> playerIdentities = new();
        foreach (var p in Players)
        {
            playerIdentities.Add(p.tempNetworkClientId, p);
        }
        GameManager.SetPlayerIdentities(playerIdentities);
    }

    public int RemovePlayer(ulong clientId)
    {
        // todo : faire ça vite !!!!
        throw new NotImplementedException("Théo a toi joué");
    }
    
    
}