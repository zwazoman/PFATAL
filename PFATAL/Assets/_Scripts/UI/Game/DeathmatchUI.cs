using System;
using TMPro;
using UnityEngine;

public class DeathmatchUI : MonoBehaviour
{
    [Header("SceneReferences")]
    [SerializeField] private TMP_Text _timerText;
    void Awake()
    {
        if (!GameManager.Instance)
        {
            Destroy(gameObject);
            return;
        }
        
        _timerText.enabled = false;
        GameManager.Instance.EventOnGameStarted += () =>
        {
            if (GameManager.gameMode is not GameManager.GameMode.DeathMatch)
            {
                Destroy(gameObject);
                return;
            }
            
            _timerText.enabled = true;
            InvokeRepeating(nameof(UpdateTimerText), 0, .5f);
        };

        GameManager.Instance.EventOnGameEnded += (_) =>
        {
            CancelInvoke(nameof(UpdateTimerText));
        };
        
    }

    void UpdateTimerText()
    {
        _timerText.text = Mathf.Ceil(
            (GameManager.DEATH_MATCH_GAME_DURATION-GameManager.Instance.TimeSinceGameStart))
            .ToString();
    }
    
}
