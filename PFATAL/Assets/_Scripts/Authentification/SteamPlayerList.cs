using System;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public struct PlayerDataSteam : INetworkSerializable, IEquatable<PlayerDataSteam>
{
    public FixedString64Bytes SteamId;
    public FixedString64Bytes PlayerName;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SteamId);
        serializer.SerializeValue(ref PlayerName);
    }
    
    public bool Equals(PlayerDataSteam other)
    {
        return SteamId.Equals(other.SteamId);
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerDataSteam other && Equals(other);
    }

    public override int GetHashCode()
    {
        return SteamId.GetHashCode();
    }
}

public class SteamPlayerList : NetworkBehaviour
{
    public static SteamPlayerList Instance;

    public NetworkList<PlayerDataSteam> Players = new NetworkList<PlayerDataSteam>();
    
    public TextMeshProUGUI  playersText;

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

        Players.OnListChanged += OnListChanged;

        Debug.Log("[SteamPlayerList] NetworkSpawn OK");
    }

    private void OnListChanged(NetworkListEvent<PlayerDataSteam> changeEvent)
    {
        Debug.Log($"[SteamPlayerList] Liste mise à jour : {Players.Count} joueur(s)");

        foreach (var p in Players)
        {
            Debug.Log($" → {p.PlayerName} | {p.SteamId}");
        }
    }
    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void AddPlayerServerRpc(string steamId, string playerName, RpcParams rpcParams = default)
    {
        foreach (var p in Players)
        {
            if (p.SteamId.ToString() == steamId)
            {
                Debug.LogWarning($"[SteamPlayerList] Joueur déjà présent : {playerName}");
                return;
            }
        }

        PlayerDataSteam data = new PlayerDataSteam
        {
            SteamId = steamId,
            PlayerName = playerName
        };

        Players.Add(data);
        playersText.text += " | " + data.SteamId + " " + data.PlayerName;
        Debug.Log($"[SteamPlayerList] Ajout : {playerName}");
    }
    
    public void RemovePlayer(ulong clientId)
    {
        if (!IsServer) return;

        for (int i = Players.Count - 1; i >= 0; i--)
        {
            Players.RemoveAt(i);
        }
    }
}