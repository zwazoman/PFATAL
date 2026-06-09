using System;
using System.Collections.Generic;
using _Scripts.Exceptions;
using _scripts.PlayerCharacter;
using Unity.Netcode;
using UnityEngine;

public class Explosion : NetworkBehaviour
{
    private static Collider[] collisionBuffer = new Collider[20];
    
    [SerializeField] public float Radius = 3f;
    [SerializeField] public float Damage = 5;
    public float KnockBackStrength;

    private const float HIT_DETECTION_DURATION = .3f;

    //synced event
    public event Action<Vector3> EventOnExplode;
        
    /// <summary>
    /// can only be called from the server
    /// </summary>
    public async Awaitable Explode(Vector3 position, ulong askerClientID, int itemID = -1)
    {
        if(!IsServer) throw new NetworkAuthorityException();
        
        CallExplosionEventRPC(position);

        //damage setup
        DamageData damageData = new();
        damageData.Amount = Damage;
        damageData.Point = position;
        damageData.SourcePos = GameManager.Instance.GetPlayerCharacter(askerClientID).transform.position;
        damageData.SourcePlayerClientID = askerClientID;
        damageData.Radius = Radius;
        damageData.WeaponID = itemID;
        
        //.2s hit detection
        float endTime = Time.time + HIT_DETECTION_DURATION;
        HashSet<DamageableObject> hitObjects = new HashSet<DamageableObject>();
        while (Time.time < endTime && isActiveAndEnabled)
        {
            TryToHitObjects(damageData, ref hitObjects);
            await Awaitable.NextFrameAsync();
        }
    }

    private void TryToHitObjects(DamageData damageData, ref HashSet<DamageableObject> alreadyHitObjects)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(damageData.Point, Radius, collisionBuffer);
        for(int i =0; i < hitCount; i++)
        {
            if(collisionBuffer[i].TryGetComponent(out DamageableObject hitObject))
            {
                //check if the object has already been hit
                if(!alreadyHitObjects.Add(hitObject)) continue;
                
                print($"{hitObject.gameObject.name} was hit by a bomb !");
                
                //direction
                damageData.Direction = (hitObject.transform.position - damageData.Point).normalized;

                //damage
                float normalizedDistance = Vector3.Distance(damageData.Point,hitObject.transform.position) / Radius;
                normalizedDistance = Mathf.Clamp(normalizedDistance, 0f, 1f);
                damageData.Amount = Damage 
                                    //* (1.0f - normalizedDistance * normalizedDistance) 
                                    * (((hitObject.OwnerClientId == damageData.SourcePlayerClientID) && hitObject.isPlayer) ? 0.3f : 1);

                //knockBack
                damageData.KnockbackForce = damageData.Direction * KnockBackStrength;

                hitObject.TakeDamage(damageData);
            }
        }
    }


    [Rpc(SendTo.Everyone)]
    void CallExplosionEventRPC(Vector3 position)
    {
        EventOnExplode?.Invoke(position);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
