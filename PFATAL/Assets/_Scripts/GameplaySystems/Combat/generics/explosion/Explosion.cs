using _Scripts.Exceptions;
using Unity.Netcode;
using UnityEngine;

public class Explosion : NetworkBehaviour
{
    private static Collider[] collisionBuffer = new Collider[20];
    
    [SerializeField] float _explosionRange = 3f;
    [SerializeField] float _damages = 5;

    
    /// <summary>
    /// can only be called from the server
    /// </summary>
    public void Explode(ulong askerClientID)
    {
        if(!IsServer) throw new NetworkAuthorityException();
        
        DamageData data = new();
        data.Amount = _damages;
        data.Point = transform.position;
        data.SourcePlayerClientID = askerClientID;
        data.Point = transform.position;
        data.Radius = _explosionRange;
        
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _explosionRange, collisionBuffer);
        for(int i =0; i < hitCount; i++)
        {
            if(collisionBuffer[i].TryGetComponent(out DamageableObject damageable))
            {
                print($"{damageable.gameObject.name} was hit by a bomb !");
                damageable.TakeDamage(data);
                
                if (damageable.TryGetComponent(out PlayerPhysics physics))
                {
                    physics.AddImpulse(physics.Position-data.Point);    
                }
            }
        }
    }
}
