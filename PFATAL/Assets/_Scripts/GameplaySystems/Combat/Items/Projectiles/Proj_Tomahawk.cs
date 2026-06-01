using FMOD.Studio;
using Unity.Netcode;
using UnityEngine;

public class Proj_Tomahawk : Proj_Falling
{
    [Header("Tomahawk Refs")]
    [SerializeField] Explosion _explosion;

    [Header("Tomahawk Settings")]
    [SerializeField] float _spinSpeed = 200;

    EventInstance _spinInstance;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        EventOnContact += Explode;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        EventOnContact -= Explode;
    }

    protected override void Update()
    {
        if(_initialized)
            visuals.transform.Rotate(_spinSpeed * Time.deltaTime,0,0);
        
        base.Update();
    }

    //[Rpc(SendTo.Server)]
    public void Explode()
    {
        _explosion.Explode(spawnContext.Value.spawnerClientID, (int)spawnContext.Value.floatData2);
    }
}
