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
        _timerText.text = Mathf.Ceil(
            (GameManager.Instance.gameSetting.GameDuration - GameManager.Instance.TimeSinceGameStart))
            .ToString();
    }
    
}
