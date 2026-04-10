using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HUDManager _hud;
    [SerializeField] Slider _slider;

    private void Start()
    {
        _hud.playerCharacter.health.OnHpChanged += SetHealthBarValue;

        SetHealthBarValue(_hud.playerCharacter.health.HP);
    }

    void SetHealthBarValue(float value)
    {
        _slider.value = value / _hud.playerCharacter.health.MaxHP;
    }
}
