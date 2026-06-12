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
            if (GameManager.Instance.gameSetting.GameMode is not GameMode.DeathMatch)
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
        int totalSeconds = Mathf.CeilToInt((GameManager.Instance.gameSetting.GameDuration - GameManager.Instance.TimeSinceGameStart));
        int seconds = totalSeconds % 60;
        int minutes = totalSeconds / 60;
        _timerText.text = minutes.ToString("D2") + ':' + seconds.ToString("D2");
    }
    
}
