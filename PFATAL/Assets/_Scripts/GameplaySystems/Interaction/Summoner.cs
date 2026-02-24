using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
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

    [SerializeField] List<GameObject> spawnableObjects;

    Dictionary<string, GameObject> spawnableObjectsDict = new();

    GameObject _currentObject;

    private void Start()
    {
        foreach(GameObject spawnableObject in spawnableObjects)
        {
            if (!spawnableObjectsDict.ContainsKey(spawnableObject.name))
            {
                spawnableObjectsDict.Add(spawnableObject.name, spawnableObject);
            }
        }
    }

    public async Awaitable<GameObject> SpawnObject(GameObject gameObject, Vector3 spawnPos, Quaternion spawnRot, SpawnContext context)
    {
        SpawnRpc(context.askerID, gameObject.name, spawnPos, spawnRot);
        while (_currentObject == null)
        {
            await Awaitable.NextFrameAsync();
        }

        GameObject newObject = _currentObject;
        _currentObject = null;

        return newObject;
    }

    public async Awaitable<GameObject> SpawnObject(string objectName, Vector3 spawnPos, Quaternion spawnRot, SpawnContext context)
    {
        return await SpawnObject(spawnableObjectsDict[objectName], spawnPos, spawnRot, context);
    }

    [Rpc(SendTo.Server)]
    void SpawnRpc(ulong askerID, string objectName, Vector3 spawnPos, Quaternion spawnRot)
    {
        NetworkObject newObject = NetworkObject.InstantiateAndSpawn(spawnableObjectsDict[objectName], NetworkManager.Singleton, 0, true, true, false, spawnPos, spawnRot);

        if (newObject.TryGetComponent(out BaseProjectile projectile))
        {
            projectile.spawnerID = askerID;
        }

        SendToAskerRpc(newObject, RpcTarget.Single(askerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SendToAskerRpc(NetworkObjectReference networkObjRef, RpcParams rpcParams = default)
    {
        if(networkObjRef.TryGet(out NetworkObject networkObj))
        {
            _currentObject = networkObj.gameObject;
        }
    }
}

public struct SpawnContext
{
    public ulong askerID;

    public SpawnContext(ulong askerID)
    {
        this.askerID = askerID;
    }
}
