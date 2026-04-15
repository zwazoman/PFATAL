using _Scripts.Pooling;
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
        DirectionIndicator indicator = LocalPoolManager.Instance.Pool_UI_DamageIndicator.
            PullObjectFromPool(transform)
            .GetComponent<DirectionIndicator>();
        
        print("indicator is null : "+(indicator == null)+
            "\nhud is null : "+(hud == null));
        
        indicator.transform.localPosition = Vector3.zero;
        
        if(damageData.Point != default)
            indicator.Setup(damageData.SourcePos, hud.playerCharacter);
        else
            indicator.Setup(Vector3.zero, hud.playerCharacter);
    }
}


