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

    private void Start()
    {
        OnDamageTaken += (_) => StopHealing();
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
        if (!_canDieFall && transform.position.y > _deathYLimit)
        {
            _canDieFall = true;
        }

        if (_canDieFall && transform.position.y < _deathYLimit && HP > 0)
        {
            _canDieFall = false;
            DamageData damageData = new();
            damageData.Amount = 100;

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
    }

}
