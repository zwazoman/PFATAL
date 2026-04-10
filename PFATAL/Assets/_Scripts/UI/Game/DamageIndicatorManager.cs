using UnityEngine;

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
        DirectionIndicator indicator = Instantiate(_prefab, transform).GetComponent<DirectionIndicator>();
        if(damageData.Point != default)
            indicator.Setup(damageData.SourcePos, hud.playerCharacter);
        else
            indicator.Setup(Vector3.zero, hud.playerCharacter);
    }
}


