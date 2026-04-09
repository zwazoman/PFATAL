using _scripts.PlayerCharacter;
using System;
using UnityEngine;

public class Proj_WolfTrap : Projectile
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _duringTime = 25f;
    [SerializeField] float _freezeDuration = 5f;
    [SerializeField] float _throwStrength = 25f;
    [SerializeField] float _activationDelay = 0.25f;
    [SerializeField] float _dammage = 1f;

    float timer;
    bool isArmed = false;
    bool hasActivated = false;
    private static Collider[] buffer = new Collider[20];

    private void Awake()
    {
        TryGetComponent(out _rb);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

        timer = 0f;
        _rb.isKinematic = false;

        Vector3 force = transform.forward * 5 + transform.up * 3;
        _rb.AddForce(force.normalized * _throwStrength, ForceMode.Impulse);
    }

    private void Update()
    {
        if (!IsSpawned || !IsServer) return;


        timer += Time.deltaTime;

        if (IsGrounded() && timer >= _freezeDuration && !isArmed)
        {
            isArmed = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
        }

        if (isArmed && !hasActivated)
        {
            HandlePlayers();
        }

        if (timer >= _duringTime)
        {
            Despawn();
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.35f);
    }

    private void HandlePlayers()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, 1, buffer);

        for (int i = 0; i < count; i++)
        {
            if (buffer[i].TryGetComponent(out DamageableObject hitObject))
            {
                DamageData damageData = new DamageData
                {
                    Amount = _dammage,
                    SourcePlayerClientID = OwnerClientId,
                    Point = hitObject.transform.position,
                    Direction = Vector3.down,
                    KnockbackForce = Vector3.zero,
                    Radius = 1
                };

                hitObject.TakeDamage(damageData);

                hitObject.TryGetComponent(out PlayerStateMachine state);
                state.s_Frozen.Freeze(_freezeDuration);

                timer = _duringTime - _freezeDuration;
                hasActivated = true;
            }
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (!isArmed || !IsServer || hasActivated) return;

        if (other.TryGetComponent(out PlayerCharacter hit))
        {
            Debug.Log("WolfTrap triggered");

            Vector3 dir = transform.position - hit.transform.position;
            float dist = dir.magnitude;

            DamageData damageData = new DamageData
            {
                Amount = _dammage,
                SourcePlayerClientID = OwnerClientId,
                Point = hit.transform.position,
                Direction = dir.normalized,
                KnockbackForce = Vector3.zero,
                Radius = 0
            };

            hit.GetComponent<DamageableObject>().TakeDamage(damageData);

            hit.stateMachine.s_Frozen.Freeze(_freezeDuration);

            timer = _duringTime - _freezeDuration;
            hasActivated = true;
        }
    }*/
}