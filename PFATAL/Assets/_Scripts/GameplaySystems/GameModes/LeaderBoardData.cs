using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardData : INetworkSerializable
{
    public SortedSet<GameRulesBase.ScoreEntry> entries = new();

    private GameRulesBase.ScoreEntry[] SerializedEntryArray;
    public LeaderBoardData()
    {
        this.entries = new();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            SerializedEntryArray = entries.ToArray();
            serializer.SerializeValue(ref SerializedEntryArray);
        }
        else
        {
            serializer.SerializeValue(ref SerializedEntryArray);
            entries.Clear();
            foreach (GameRulesBase.ScoreEntry entry in SerializedEntryArray)
            {
                entries.Add(entry);
            }
        }
        
    }
}
