using _scripts.PlayerCharacter;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] PlayerCharacter _main;

    [Header("References")]
    [SerializeField] public GameObject deathUi;
    [SerializeField] public Button respawnButton;

    public void ShowDeathUI()
    {
        deathUi.SetActive(true);
    }

    public void HideDeathUI()
    {
        deathUi.SetActive(false);
    }
}
