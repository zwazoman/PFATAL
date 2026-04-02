using UnityEngine;
using System.Collections.Generic;

public class DamageIndicatorManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public HUDManager hud;
    [SerializeField] GameObject _prefab;

    private void Start()
    {
        hud.playerCharacter.health.OnDamageTaken += ShowIndicator;
    }

    public void ShowIndicator(DamageData damageData)
    {
        DamageIndicator indicator = Instantiate(_prefab, transform).GetComponent<DamageIndicator>();
        indicator._manager = this;
        if(damageData.Point != default)
            indicator.damagePosition = damageData.SourcePos;
        else
            indicator.damagePosition = Vector3.zero;
    }
}


