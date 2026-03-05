using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardData : INetworkSerializable
{
    public SortedSet<GameRulesBase.ScoreEntry> entries = new();

    private GameRulesBase.ScoreEntry[] _serializedEntryArray;
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
            foreach (GameRulesBase.ScoreEntry entry in _serializedEntryArray)
            {
                entries.Add(entry);
            }
        }
        
    }

    public void Clear()
    {
        entries.Clear();
        _serializedEntryArray = Array.Empty<GameRulesBase.ScoreEntry>();
    }
}
