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
    
    protected override void Update()
    {
        if(_initialized)
            visuals.transform.Rotate(_spinSpeed * Time.deltaTime,0,0);
        
        base.Update();
    }

    //[Rpc(SendTo.Server)]
    protected override void OnContact(RaycastHit sourceHit)
    {
        print("contact !");
        Explode(sourceHit.point);
    }

    public void Explode(Vector3 position)
    {
        _explosion.transform.position = position;
        _explosion.Explode( position,spawnContext.Value.spawnerClientID, (int)spawnContext.Value.floatData2);
    }
}
