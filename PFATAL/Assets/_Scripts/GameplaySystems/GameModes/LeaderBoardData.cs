using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardData : INetworkSerializable
{
    public SortedSet<ScoreEntry> entries = new();

    private ScoreEntry[] _serializedEntryArray;
    public LeaderBoardData()
    {
        this.entries = new();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            _serializedEntryArray = entries.ToArray();
            serializer.SerializeValue(ref _serializedEntryArray);
        }
        else
        {
            serializer.SerializeValue(ref _serializedEntryArray);
            entries.Clear();
            foreach (ScoreEntry entry in _serializedEntryArray)
            {
                entries.Add(entry);
            }
        }
        
    }
    
    /// <summary>
    /// Résout les noms des joueurs depuis SteamPlayerList avant affichage.
    /// À appeler côté UI juste avant de lire les entrées.
    /// </summary>
    public void ResolvePlayerNames()
    {
        Debug.Log("[LeaderBoard] ResolvePlayerNames appelé ici!");
        var resolved = entries.ToList();
        entries.Clear();
        foreach (var entry in resolved)
        {
            string name = "Player_" + entry.ClientID;

            try
            {
                PermanentPlayerIdentity identity = GameManager.GetPlayerIdentity(entry.ClientID);
                if (!string.IsNullOrEmpty(identity.name))
                    name = identity.name;
                Debug.Log($"[LeaderBoard] Trouvé via GameManager : {name}");
            }
            catch
            {
                Debug.LogWarning($"[LeaderBoard] Pas d'identité trouvée pour clientID={entry.ClientID}");
            }

            entries.Add(new ScoreEntry(entry.ClientID, entry.Rank, entry.Kills, entry.Deaths, entry.Points, name));
        }
    }

    public void Clear()
    {
        entries.Clear();
        _serializedEntryArray = Array.Empty<ScoreEntry>();
    }

    public override string ToString()
    {
        string s = "";
        s+="leader board entry count : "+entries.Count+'\n';
        foreach (var entry in entries)
        {
            s +=
                entry.ToString() + '\n';
        }

        return s;
    }
}

public struct ScoreEntry : IComparable<ScoreEntry>, INetworkSerializeByMemcpy
{
    public ulong ClientID;
    public int Rank, Kills, Deaths, Points;
    public FixedString64Bytes PlayerName; // todo : ajouter player name → récupérer via GameLobby.Instance ou SteamPlayerList.Instance

    public int CompareTo(ScoreEntry other)
    {
        int comparisonResult = other.Points.CompareTo(Points); //compare les scores
        if (comparisonResult == 0)
        {
            comparisonResult = other.Kills.CompareTo(other.Kills); //puis les kills si le score est egal
            if (comparisonResult == 0)
            {
                comparisonResult = -(other.Deaths.CompareTo(other.Deaths)); //puis les morts si les kills sont egaux
                
                if(comparisonResult == 0) 
                    comparisonResult = other.ClientID.CompareTo(other.ClientID); //puis les clients ids si c'est toujours egal
            }
        }
        
        return comparisonResult; 
    }

    public ScoreEntry(ulong clientID, int rank, int kills, int deaths, int points, string playerName = "")
    {
        ClientID = clientID;
        Rank = rank;
        Kills = kills;
        Deaths = deaths;
        Points = points;
        PlayerName = playerName;
    }

    public override string ToString()
    {
        const string space = " | ";
        return "Player : " + PlayerName + " (" + ClientID + ")" + space +
               "Kills : " + Kills + space +
               "Deaths : " + Deaths + space +
               "Points : " + Points + space +
               "Rank : " + Rank;
    }
}