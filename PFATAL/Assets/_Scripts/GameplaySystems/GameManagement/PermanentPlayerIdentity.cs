
using Unity.Netcode;

/// <summary>
/// l'identité permanente du joueur (steam...)
/// </summary>
public struct  PermanentPlayerIdentity : INetworkSerializable
{
    public string name;
    public string platformID;
    public ePlatform platform;
    public ulong tempNetworkClientId;
    
    public enum ePlatform
    {
        None,
        Steam,
        Unity,
    }

    public PermanentPlayerIdentity(string name, string platformID, ePlatform platform, ulong tempNetworkClientId)
    {
        this.name = name;
        this.platformID = platformID;
        this.platform = platform;
        this.tempNetworkClientId = tempNetworkClientId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref platformID);
        serializer.SerializeValue(ref platform);
        serializer.SerializeValue(ref tempNetworkClientId);
    }
}
