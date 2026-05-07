using _scripts.PlayerCharacter;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public event Action<bool> OnTriggerHitFeedback;

    [SerializeField] public PlayerCharacter playerCharacter;

    [Header("References")]
    [SerializeField] public GameObject deathUi;
    [SerializeField] public Button respawnButton;
    [SerializeField] HitMarkerUI hitMarkerUI;

    public void TriggerHitFeedback(bool isDead)
    {
        OnTriggerHitFeedback?.Invoke(isDead);
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
