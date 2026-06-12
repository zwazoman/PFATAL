using System;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class Proj_WolfTrap : Projectile
{
    public event Action OnTrapPlayer;
    public event Action OnDeploy;

    [SerializeField] Rigidbody _rb;
    [SerializeField] float _duringTime = 25f;
    [SerializeField] float _freezeDuration = 5f;
    [SerializeField] float _throwStrength = 25f;
    [SerializeField] float _activationDelay = 0.5f;
    [SerializeField] float _dammage = 1f;
    [SerializeField] float _radius = 1;
    [SerializeField] LayerMask _groundCheckMask;
    [SerializeField] ParticleSystem _particleSmoke;

    float timer;
    bool isArmed = false;
    bool hasActivated = false;
    private static Collider[] buffer = new Collider[20];
    
    private Tween _scaleTween;

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

    public override void Despawn()
    {
        PlayParticleSystemRpc(true);
        
        _scaleTween = transform.DOScale(0f, 1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            base.Despawn();
        });
    }

    private void Update()
    {
        if (!IsSpawned || !IsServer) return;


        timer += Time.deltaTime;

        if (IsGrounded() && timer >= _activationDelay && !isArmed)
        {
            isArmed = true;
            PlayParticleSystemRpc();
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
            OnDeploy?.Invoke();
        }

        if (isArmed && !hasActivated)
        {
            HandlePlayers();
        }

        if (timer >= _duringTime || GetComponent<DamageableObject>().IsDead)
        {
            Despawn();
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, _groundCheckMask );
    }

    private void HandlePlayers()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, _radius, buffer);

        for (int i = 0; i < count; i++)
        {
            if (!buffer[i].TryGetComponent(out DamageableObject hitObject) || !hitObject.isPlayer) continue;


            // D�g�ts c�t� serveur
            DamageData damageData = new DamageData
            {
                Amount = _dammage,
                SourcePlayerClientID = OwnerClientId,
                SourcePos = transform.position,
                Point = hitObject.transform.position,
                Direction = Vector3.down,
                KnockbackForce = Vector3.zero,
                Radius = _radius,
                WeaponID = (int)spawnContext.Value.floatData2
            };

            // RPC vers le client cibl�
            ulong targetClientId = hitObject.NetworkObject.OwnerClientId;
            ApplyFreezeRPC(_freezeDuration, RpcTarget.Single(targetClientId, RpcTargetUse.Temp));
            hitObject.TakeDamage(damageData);

            OnTrapPlayer?.Invoke();

            timer = _duringTime - _freezeDuration;
            hasActivated = true;
            break;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void ApplyFreezeRPC(float duration, RpcParams rpcParams = default)
    {
        var player = GameManager.Instance.localPlayerCharacter;

        if (player == null)
        {
            Debug.LogError("[WolfTrap RPC] localPlayerCharacter est null");
            return;
        }

        Debug.Log($"[WolfTrap RPC] APPLY FREEZE sur client {NetworkManager.Singleton.LocalClientId}");
        player.stateMachine.s_Frozen.Freeze(duration);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    [Rpc(SendTo.Everyone)]
    private void PlayParticleSystemRpc(bool destroy = false)
    {
        if (_particleSmoke == null || _particleSmoke.isPlaying) return;

        if (destroy)
            _particleSmoke.transform.SetParent(null);

        _particleSmoke.Play(true);
    }
}