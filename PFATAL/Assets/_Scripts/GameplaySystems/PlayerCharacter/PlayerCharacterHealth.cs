using System.Threading;
using UnityEngine;

public class PlayerCharacterHealth : DamageableObject
{
    [Header("Auto Heal Settings")]
    [SerializeField] float _regenDelay = .3f;
    [SerializeField] float _regenStrength = 1f;
    [SerializeField] float _regenCooldown = 3f;
    [SerializeField] float _deathYLimit = -10f;

    bool _heal = false;
    bool _canDieFall = true;
    float _regenTickTimer;
    float _regenCooldownTimer;

    [Header("Damage Visual")]
    [SerializeField] Renderer[] _characterRenderers;

    MaterialPropertyBlock _mpb;
    int _healthID;

    private void Start()
    {
        OnDamageTaken += (_) => StopHealing();
        _mpb = new MaterialPropertyBlock();
        _healthID = Shader.PropertyToID("_Health");
    }

    void StartHealing()
    {
        _heal= true;
        _regenCooldownTimer = 0;
    }

    void StopHealing()
    {
        _heal = false;
        _regenTickTimer = 0;
        _regenCooldownTimer = 0;
    }

    private void Update()
    {
        if (!_canDieFall && transform.position.y > _deathYLimit && IsOwner)
        {
            _canDieFall = true;
        }

        if (_canDieFall && transform.position.y < _deathYLimit && HP > 0 && IsOwner)
        {
            _canDieFall = false;
            DamageData damageData = new();
            damageData.Amount = MaxHP;
            damageData.SourcePlayerClientID = DamageData.NON_PLAYER_DAMAGE_SOURCE_CLIENT_ID;
            damageData.SourcePos = transform.position;

            TakeDamage(damageData);
            return;
        }

        if (_heal)
        {
            _regenTickTimer += Time.deltaTime;
            if (_regenTickTimer >= _regenDelay)
            {
                Heal(_regenStrength);
                _regenTickTimer = 0;

                if (HP == MaxHP)
                    StopHealing();
            }
        }
        else if(HP > 0 && HP < MaxHP)
        {
            _regenCooldownTimer += Time.deltaTime;
            if(_regenCooldownTimer >= _regenCooldown)
            {
                StartHealing();
            }
        }

        if (MaxHP > 0)
        {
            float health = HP / MaxHP;
            foreach (var r in _characterRenderers)
            {
                r.GetPropertyBlock(_mpb);
                _mpb.SetFloat(_healthID, health);
                r.SetPropertyBlock(_mpb);
            }
        }
    }
}
