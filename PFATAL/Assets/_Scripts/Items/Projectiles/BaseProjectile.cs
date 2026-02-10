using Unity.Netcode;
using UnityEngine;

public class BaseProjectile : NetworkBehaviour
{
    [SerializeField] float _speed = 50;
    [SerializeField] float _lifetime = 3;

    bool _move;
    float _timer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();


    }

    private void Update()
    {
        if (!IsSpawned || !IsServer)
            return;

        transform.Translate(transform.forward * _speed * Time.deltaTime,Space.World);

        _timer += Time.deltaTime;

        if (_timer >= _lifetime)
        {
            NetworkObject.Despawn();
            _timer = 0;
        }
    }

    //[Rpc(SendTo.Server)]
    //void DespawnRpc()
    //{
    //    NetworkObject.Despawn();
    //}
}
