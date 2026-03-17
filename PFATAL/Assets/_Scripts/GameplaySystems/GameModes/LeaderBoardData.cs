using System;
using System.Collections.Generic;
using System.Linq;
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

public struct ScoreEntry :IComparable<ScoreEntry>, INetworkSerializeByMemcpy
{
    public ulong ClientID;
    public int Rank,Kills,Deaths,Points;
    public int CompareTo(ScoreEntry other)
    {
        int result = other.Points.CompareTo(Points);
        return result !=0 ? result : ClientID.CompareTo(other.ClientID);
    }

    public ScoreEntry(ulong clientID, int rank, int kills, int deaths, int points)
    {
        ClientID = clientID;
        Rank = rank;
        Kills = kills;
        Deaths = deaths;
        Points = points;
    }

    public override string ToString()
    {
        const string space = " | ";
        return "Player : " + ClientID + space +
               "Kills : " + Kills + space +
               "Deaths : " + Deaths + space +
               "Points : " + Points + space +
               "Rank : " + Rank;
    }
}