
using _scripts.PlayerCharacter;
using _Scripts.Pooling;
using UnityEngine;

public class PlayerCharacterVisuals : MonoBehaviour
{
    [Header("scene references")]
    [SerializeField] PlayerCharacter _playerCharacter;
    
    void Awake()
    {
        _playerCharacter.health.OnDamageTaken += OnDamageTaken;
    }

    private void OnDamageTaken(DamageData damageData)
    {
        //vfx
        LocalPoolManager.Instance.Pool_VFX_Hit_crit.
            PullObjectFromPool(damageData.Point, Quaternion.LookRotation(-damageData.Direction))
            .GoBackIntoPool_Delayed(1.5f);
        
        
    }
}
