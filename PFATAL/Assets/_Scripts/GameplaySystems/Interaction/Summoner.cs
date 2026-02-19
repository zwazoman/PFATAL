using AYellowpaper.SerializedCollections;
using Unity.Netcode;
using UnityEngine;

public class Summoner : NetworkBehaviour
{
    #region Singleton
    private static Summoner instance;

    public static Summoner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Summoner");
                instance = go.AddComponent<Summoner>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }
    #endregion

    [SerializedDictionary] public SerializedDictionary<string, GameObject> objects;

    [Rpc(SendTo.Server)]
    public void ShootRpc(string projectileName, Vector3 spawnPos, Quaternion spawnRot)
    {
        NetworkObject.InstantiateAndSpawn(objects[projectileName], NetworkManager.Singleton, 0, true, true, false, spawnPos, spawnRot);
    }
}
