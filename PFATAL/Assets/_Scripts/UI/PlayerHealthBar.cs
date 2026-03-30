using _scripts.PlayerCharacter;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] Slider _slider;

    private void Start()
    {
        _playerCharacter.health.OnHpChanged += SetHealthBarValue;

        SetHealthBarValue(_playerCharacter.health.HP);
    }

    void SetHealthBarValue(float value)
    {
        _slider.value = value / _playerCharacter.health.MaxHP;
    }
}
