using System;
using _scripts.PlayerCharacter;
using NUnit.Framework;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using static DG.Tweening.DOTweenModuleUtils;

public class DamageableObject : NetworkBehaviour, IDamageable
{
    /// <summary>
    /// Le client ID du dernier joueur qui a provoqué les dégats. DamageData.NON_PLAYER_DAMAGE_SOURCE_CLIENT_ID -> dégats pas provoqués par un joueur (piège...)
    /// </summary>
    public ulong LastDamageSourceClientID { get; private set; }
    public float HP {get; private set;}
    [field:SerializeField] public float MaxHP { get; private set; }
    public bool IsDead => HP == 0;

    [SerializeField] public bool isPlayer = true;
    //events
    public event Action<DamageData> OnDamageTaken;
    public event Action OnDie;
    public event Action<float> OnHpChanged;

    void Awake()
    {
        HP = MaxHP;
        OnHpChanged?.Invoke(HP);
    }

    public void SetMaxHP(float newMax)
    {
        SetMaxHPRpc(newMax);
    }

    /// <summary>
    /// Fait des dégats à l'entité. Doit être appelé sur le serveur uniquement.
    /// </summary>
    public void TakeDamage(DamageData damageData)
    {
        if (IsDead) return;
        
        LastDamageSourceClientID = damageData.SourcePlayerClientID;
        SetHpRPC(HP - damageData.Amount);
        InvokeDamageEventRPC(damageData);

        //knockback
        if (TryGetComponent(out PlayerCharacter player) && damageData.KnockbackForce != Vector3.zero)
            ApplyKnockbackRpc(damageData, RpcTarget.Single(player.OwnerClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void ApplyKnockbackRpc(DamageData data, RpcParams rpcParams = default)
    {
        TryGetComponent(out PlayerPhysics physics);
        physics.AddImpulse(data.KnockbackForce);
    }

    /// <summary>
    /// Soigne entièrement l'entité. Doit être appelé sur le serveur uniquement.
    /// </summary>
    public void Heal()
    {
        SetHpRPC(MaxHP);
    }
    
    /// <summary>
    /// Soigne l'entité. Doit être appelé sur le serveur uniquement.
    /// </summary>
    public void Heal(float amount)
    {
        SetHpRPC(HP + amount);
    }
    
    //replication 
    [Rpc(SendTo.Everyone)]
    private void SetHpRPC(float hp)
    {
        HP = Mathf.Clamp(hp,0,MaxHP);
        OnHpChanged?.Invoke(HP);

        if (HP == 0)
        {
            OnDie?.Invoke();
            print("die");
        }
    }

    [Rpc(SendTo.Everyone)]
    void SetMaxHPRpc(float value)
    {
        MaxHP = value;
        HP = MaxHP;
        OnHpChanged?.Invoke(HP);
    }
    
    [Rpc(SendTo.Everyone)]
    public void InvokeDamageEventRPC(DamageData damageData)
    {
        OnDamageTaken?.Invoke(damageData);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DamageableObject))]
public class DamageableObjectEditor : Editor
{
    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        GUILayout.Label("HPs : "+((DamageableObject)target).HP);
    }
}
#endif
