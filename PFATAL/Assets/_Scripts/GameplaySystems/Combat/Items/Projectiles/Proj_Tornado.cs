using System.Collections;
using System.Collections.Generic;
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
    private List<DamageableObject> damageableObjects = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
    }

    private void Update()
    {
        if (!IsSpawned || !IsServer) return;

        transform.position += transform.forward * _speed * Time.deltaTime;

        _timer += Time.deltaTime;

        if (_timer >= 1f)
        {
            HandlePlayers();

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

    void HandlePlayers()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, _ejectionRadius, buffer);

        for (int i = 0; i < count; i++)
        {
            if (buffer[i].TryGetComponent(out DamageableObject hitObject))
            {
                Vector3 dir = transform.position - hitObject.transform.position;
                float dist = dir.magnitude;
                Vector3 knockbackForce;

                if (!damageableObjects.Contains(hitObject))
                {
                    Vector3 eject = hitObject.transform.up * _ejectionForce;

                    knockbackForce = eject;

                    DamageData damageData = new DamageData
                    {
                        Amount = 0,
                        SourcePlayerClientID = OwnerClientId,
                        Point = hitObject.transform.position,
                        Direction = dir.normalized,
                        KnockbackForce = knockbackForce,
                        Radius = _ejectionRadius
                    };

                    damageableObjects.Add(hitObject);
                    StartCoroutine(DeleteToList(hitObject));

                    hitObject.TryGetComponent(out Rigidbody rb);
                    rb.linearVelocity = Vector3.zero;

                    hitObject.TakeDamage(damageData);

                    hitObject.TryGetComponent(out PlayerStateMachine playerStateMachine);
                    playerStateMachine.s_PropulseInAir.ActivateState(OwnerClientId);
                }
            }
        }
    }

    IEnumerator DeleteToList(DamageableObject damageableObject)
    {
        yield return new WaitForSeconds(1f);
        damageableObjects.Remove(damageableObject);
    }
}
