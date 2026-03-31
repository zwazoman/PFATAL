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
            if (buffer[i].TryGetComponent(out DamageableObject hitobject))
            {
                Vector3 dir = transform.position - hitobject.transform.position;
                float dist = dir.magnitude;
                Vector3 knockbackForce;

                if (!damageableObjects.Contains(hitobject))
                {
                    Vector3 eject = hitobject.transform.up * _ejectionForce;

                    knockbackForce = eject;

                    DamageData damageData = new DamageData
                    {
                        Amount = 0,
                        SourcePlayerClientID = OwnerClientId,
                        Point = hitobject.transform.position,
                        Direction = dir.normalized,
                        KnockbackForce = knockbackForce,
                        Radius = _ejectionRadius
                    };

                    damageableObjects.Add(hitobject);
                    StartCoroutine(DeleteToList(hitobject));
                    Rigidbody rb = hitobject.GetComponent<Rigidbody>();
                    rb.linearVelocity = Vector3.zero;
                    hitobject.TakeDamage(damageData);
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
