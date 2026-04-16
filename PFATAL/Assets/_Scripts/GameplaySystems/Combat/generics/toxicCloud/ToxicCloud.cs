using System;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class ToxicCloud : NetworkBehaviour
{
    public event Action OnSmokeStart;
    public event Action OnSmokeEnd;

    private static Collider[] buffer = new Collider[20];

    [SerializeField] public float radius = 4f;
    [SerializeField] float _duration = 6f;
    [SerializeField] float _tickRate = 0.5f;
    [SerializeField] float _damagePerTick = 1f;

    float _timer;
    float _tickTimer;

    ulong _ownerId;
    
    const float TWEEN_DURATION = .4f;
    private bool _isAboutToDie = false;
    public void Init(ulong ownerId)
    {
        _ownerId = ownerId;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        //end tween
        if (!_isAboutToDie && _timer >= _duration-TWEEN_DURATION)
        {
            _isAboutToDie = true;
            transform.DOScale(Vector3.zero, TWEEN_DURATION).SetEase(Ease.InCirc);
            DOTween.To(()=> radius,(float v)=>radius = v,0,TWEEN_DURATION).SetEase(Ease.InCirc);
        }
        
        //== server only ==
        
        if (!IsServer) return;
        
        _tickTimer += Time.deltaTime;

        if (_tickTimer >= _tickRate)
        {
            _tickTimer = 0f;
            ApplyDamageToOverlappingPlayers();
        }
        
        if (_timer >= _duration)
        {
            BroadcastSmokeEndRpc();
            NetworkObject.Despawn();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        print("debout connard");
        BroadcastSmokeStartRpc();

        //spawn tween
        transform.localScale = Vector3.one*.2f;
        transform.DOScale(new Vector3(radius, radius, radius), TWEEN_DURATION).SetEase(Ease.OutCubic);

        float endRadius = radius; radius = 0;
        DOTween.To(()=> radius,(float v)=>radius = v,endRadius,TWEEN_DURATION).SetEase(Ease.OutElastic);
    }

    void ApplyDamageToOverlappingPlayers()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, buffer);

        for (int i = 0; i < count; i++)
        {
            if (buffer[i].TryGetComponent(out DamageableObject hit))
            {
                DamageData damage = new DamageData
                {
                    Amount = _damagePerTick,
                    SourcePlayerClientID = _ownerId,
                    Point = hit.transform.position,
                    Direction = Vector3.zero,
                    KnockbackForce = Vector3.zero,
                    Radius = radius
                };

                hit.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    [Rpc(SendTo.Everyone)]
    void BroadcastSmokeStartRpc() => OnSmokeStart?.Invoke();

    [Rpc(SendTo.Everyone)]
    void BroadcastSmokeEndRpc() => OnSmokeEnd?.Invoke();
}
