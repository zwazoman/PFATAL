using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Proj_Tornado : Projectile
{
    [Header("Settings")]
    [SerializeField] float _duringTime = 5f;
    [SerializeField] float _speed = 4f;
    [SerializeField] float _ejectionRadius = 2f;
    [SerializeField] float _ejectionForce = 10f;

    private float _timer;
    private bool _halfTimeReached = false;
    private static Collider[] buffer = new Collider[20];
    private List<DamageableObject> blackList = new();

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _ejectionRadius);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
    }

    private void Update()
    {
        //todo : synchoniser déplacements client et server avec le timestamp et l'override de la méthode computePosition(float timestamp...) plutot qu'avec le network transform
        
        if (!IsSpawned || !IsServer) return;

        transform.position += transform.forward * (_speed * Time.deltaTime);

        _timer += Time.deltaTime;

        if (_timer >= 1f)
        {
            CheckForCollisionsAgainstPlayers();

            if (_timer >= _duringTime/2 && !_halfTimeReached)
            {
                _speed /= 2;
                _halfTimeReached = true;
            }
            if (_timer >= _duringTime)
            {
                Despawn();
            }
        }
    }

    void CheckForCollisionsAgainstPlayers()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, _ejectionRadius, buffer);

        for (int i = 0; i < count; i++)
        {
            if (buffer[i].TryGetComponent(out DamageableObject hitObject))
            {
                

                if (!blackList.Contains(hitObject))
                {
                    Vector3 dir = transform.position - hitObject.transform.position;
                    float dist = dir.magnitude;
                    
                    Vector3 playerVelocity = GameManager.Instance.GetPlayerCharacter(hitObject.OwnerClientId).physics.Velocity;
                    Vector3 velocityAfterKnockback = new Vector3(playerVelocity.x,_ejectionForce,playerVelocity.z);
                    Vector3 knockback = velocityAfterKnockback-playerVelocity;
                    
                    DamageData damageData = new DamageData
                    {
                        Amount = 0,
                        SourcePlayerClientID = OwnerClientId,
                        Point = hitObject.transform.position,
                        Direction = dir.normalized,
                        KnockbackForce = knockback,
                        Radius = _ejectionRadius
                    };
                    hitObject.TakeDamage(damageData);

                    blackList.Add(hitObject);
                    StartCoroutine(RemoveObjectFromBlackList_Delayed(hitObject));

                    if( hitObject.TryGetComponent(out PlayerStateMachine playerStateMachine));
                    playerStateMachine.s_PropulseInAir.ActivateState(OwnerClientId);
                }
            }
        }
    }

    IEnumerator RemoveObjectFromBlackList_Delayed(DamageableObject damageableObject)
    {
        yield return new WaitForSeconds(1f);
        blackList.Remove(damageableObject);
    }
}
