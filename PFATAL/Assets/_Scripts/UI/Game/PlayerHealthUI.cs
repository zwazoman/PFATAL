using _scripts.PlayerCharacter;
using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] PlayerCharacter main;

    [SerializeField] TMP_Text _healthText;

    string connard;

    private void Awake()
    {
        TryGetComponent(out _healthText);
    }

    private void Start()
    {
        connard = $" / {main.health.MaxHP}";

        _healthText.text = main.health.MaxHP + connard;

        main.health.OnDamageTaken += (_) => SetHealthText();
    }

    void SetHealthText()
    {
        _healthText.text = main.health.HP + connard;
    }

}
