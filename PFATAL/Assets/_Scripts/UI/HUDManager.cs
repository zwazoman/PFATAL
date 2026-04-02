using _scripts.PlayerCharacter;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] public PlayerCharacter playerCharacter;

    [Header("References")]
    [SerializeField] public GameObject deathUi;
    [SerializeField] public Button respawnButton;
    [SerializeField] HitMarkerUI hitMarkerUI;

    public void TriggerHitFeedback()
    {
        AudioManager.Instance.PlayOneShot(Sounds.HitMarker);

        //hitMarkerUI.ShowHitMarker();
    }

    public void ShowDeathUI()
    {
        deathUi.SetActive(true);
    }

    public void HideDeathUI()
    {
        deathUi.SetActive(false);
    }
}
