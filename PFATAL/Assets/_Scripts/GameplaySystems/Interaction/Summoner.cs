using _scripts.PlayerCharacter;
using System;
using System.Collections.Generic;
using NetworkTime;
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObjectToSpawn"></param>
    /// <param name="spawnPos"></param>
    /// <param name="spawnRot"></param>
    /// <param name="context"></param>
    /// <param name="giveOwnershipToAsker"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Awaitable<GameObject> SpawnObject(GameObject gameObjectToSpawn, Vector3 spawnPos, Quaternion spawnRot, bool sendBack, SpawnContext? context = null, ulong futureOwner = 1000)
    {
        if (context == null)
            context = new(0);

        if (gameObjectToSpawn == null)
            throw new ArgumentNullException(nameof(gameObjectToSpawn));

        SpawnRpc(context.Value, gameObjectToSpawn.name, spawnPos, spawnRot, futureOwner, sendBack);
        while (_currentObject == null && sendBack)
        {
            await Awaitable.NextFrameAsync();
        }

        GameObject newObject = _currentObject;
        _currentObject = null;

        return sendBack ? newObject : null;
    }

    [Rpc(SendTo.Server)]
    void SpawnRpc(SpawnContext context, string objectName, Vector3 spawnPos, Quaternion spawnRot, ulong futureOwner, bool sendBack)
    {
        //todo => refaire avec les pools bien

        NetworkObject newObject = null;

        if (futureOwner == 1000)
            newObject = NetworkObject.InstantiateAndSpawn(spawnableObjectsDict[objectName], NetworkManager.Singleton, 0, false, false, false, spawnPos, spawnRot);
        else
            newObject = NetworkObject.InstantiateAndSpawn(spawnableObjectsDict[objectName], NetworkManager.Singleton, futureOwner, false, false, false, spawnPos, spawnRot);

        context.spawnPos = spawnPos;
        context.spawnTime = TimeStamp.Now;

        //newObject = Instantiate(spawnableObjectsDict[objectName], spawnPos, spawnRot).GetComponent<NetworkObject>();

        if (newObject.TryGetComponent(out Projectile projectile))
        {
            projectile.spawnContext = new(context);
            projectile.OnSpawn();
        }

        //if (futureOwner == 1000)
        //    newObject.Spawn();
        //else
        //    newObject.SpawnWithOwnership(futureOwner);

        if (sendBack)
            SendToAskerRpc(newObject, RpcTarget.Single(context.spawnerClientID, RpcTargetUse.Temp));
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
    public ulong spawnerClientID;
    public float floatData;
    public float floatData2;
    public Vector3 spawnPos;
    public float spawnTime;

    public SpawnContext(ulong spawnerClientID) : this()
    {
        this.spawnerClientID = spawnerClientID;
        floatData = 0;
        floatData2 = 0;
        spawnTime = TimeStamp.Now;
    }
}
