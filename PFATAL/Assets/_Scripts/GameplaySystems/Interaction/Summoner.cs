using AYellowpaper.SerializedCollections;
using System;
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
    #endregion

    [SerializeField] NetworkPrefabsList prefabs;

    Dictionary<string, GameObject> spawnableObjectsDict = new();
    GameObject _currentObject;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);


        foreach (NetworkPrefab spawnableObject in prefabs.PrefabList)
        {
            if (!spawnableObjectsDict.ContainsKey(spawnableObject.Prefab.name))
            {
                spawnableObjectsDict.Add(spawnableObject.Prefab.name, spawnableObject.Prefab);
            }
        }
    }

    public async Awaitable<GameObject> SpawnObject(GameObject gameObjectToSpawn, Vector3 spawnPos, Quaternion spawnRot, SpawnContext? context = null, bool giveOwnershipToAsker = false)
    {
        if (context == null)
            context = new(0);
        
        if(gameObjectToSpawn == null)
            throw new ArgumentNullException(nameof(gameObjectToSpawn));
        
        SpawnRpc(context.Value, gameObjectToSpawn.name, spawnPos, spawnRot, giveOwnershipToAsker);
        while (_currentObject == null)
        {
            await Awaitable.NextFrameAsync();
        }

        GameObject newObject = _currentObject;
        _currentObject = null;

        return newObject;
    }

    public async Awaitable<GameObject> SpawnObject(string objectName, Vector3 spawnPos, Quaternion spawnRot, SpawnContext? context = null)
    {
        return await SpawnObject(spawnableObjectsDict[objectName], spawnPos, spawnRot, context);
    }

    [Rpc(SendTo.Server)]
    void SpawnRpc(SpawnContext context, string objectName, Vector3 spawnPos, Quaternion spawnRot, bool giveOwnershipToAsker)
    {
        NetworkObject newObject = null;

        if (giveOwnershipToAsker)
            newObject = NetworkObject.InstantiateAndSpawn(spawnableObjectsDict[objectName], NetworkManager.Singleton, context.askerID, true, true, false, spawnPos, spawnRot);
        else
            newObject = NetworkObject.InstantiateAndSpawn(spawnableObjectsDict[objectName], NetworkManager.Singleton, 0, true, true, false, spawnPos, spawnRot);

        if (newObject.TryGetComponent(out Projectile projectile))
        {
            projectile.spawnContext = context;
            projectile.OnSpawn();
        }

        SendToAskerRpc(newObject, RpcTarget.Single(context.askerID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SendToAskerRpc(NetworkObjectReference networkObjRef, RpcParams rpcParams = default)
    {
        if (networkObjRef.TryGet(out NetworkObject networkObj))
        {
            _currentObject = networkObj.gameObject;
        }
    }
}


public struct SpawnContext : INetworkSerializeByMemcpy
{
    public ulong askerID;
    public float data;

    public SpawnContext(ulong askerID) :this()
    {
        this.askerID = askerID;
    }
}
