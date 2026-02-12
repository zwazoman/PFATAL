using System;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class DamageableObject : NetworkBehaviour, IDamageable
{
    public int LastDamageSourceClientID { get; private set; }
    public float HP {get; private set;}
    [field:SerializeField] public float MaxHP { get; private set; }
    public bool IsDead => HP == 0;
    
    //events
    public event Action<DamageData> OnDamageTaken;
    public event Action OnDead;
    public event Action<float> OnHpChanged;
    
    /// <summary>
    /// Fait des dégats à l'entité. Doit être appelé sur le serveur uniquement.
    /// </summary>
    public void TakeDamage(DamageData damageData)
    {
        Assert.IsTrue(IsServer,"Impossible d'appliquer les dégats depuis un client.");
        
        LastDamageSourceClientID = damageData.SourcePlayerClientID;
        SetHpRPC(HP - damageData.Amount);
        InvokeDamageEventRPC(damageData);
    }

    /// <summary>
    /// Soigne entièrement l'entité. Doit être appelé sur le serveur uniquement.
    /// </summary>
    public void Heal()
    {
        Assert.IsTrue(IsServer,"Impossible de modifier les HPs depuis un client.");
        SetHpRPC(MaxHP);
    }
    
    /// <summary>
    /// Soigne l'entité. Doit être appelé sur le serveur uniquement.
    /// </summary>
    public void Heal(float amount)
    {
        Assert.IsTrue(IsServer,"Impossible de modifier les HPs depuis un client.");
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
            OnDead?.Invoke();
        }
    }
    
    [Rpc(SendTo.Everyone)]
    public void InvokeDamageEventRPC(DamageData damageData)
    {
        OnDamageTaken?.Invoke(damageData);
    }
}
