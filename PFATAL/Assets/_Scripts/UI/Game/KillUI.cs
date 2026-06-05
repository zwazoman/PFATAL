using _Scripts.Pooling;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HUDManager _hudManager;
    [SerializeField] float topLimit, botLimit, rightLimit, leftLimit;

    [Header("Settings")]
    [SerializeField] string _feedEndText;
    [SerializeField] float _textscalePunchValue = 5;
    [SerializeField] float _textScalePunchDuration = .5f;
    [SerializeField] float _textLifeTime = 2.5f;

    int _textCount = 0;

    private void Start()
    {
        _hudManager.OnTriggerHitFeedback += (bool dead, string killedPlayerName) => { if (dead) HitFeedback_Callback(killedPlayerName); };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            HitFeedback_Callback("GROS CONNARD");
        }
    }

    void HitFeedback_Callback(string killedPlayerName)
    {
        _textCount++;

        PooledObject pooledText = LocalPoolManager.Instance.Pool_UI_Kill.PullObjectFromPool(transform);

        //todo => faut gérer la position en fonction du nombre. placer en dessous en fonction du nombre affichés
        //layout group juste ? prendre des refs
        pooledText.transform.localPosition = new Vector2(Random.Range(leftLimit, rightLimit), Random.Range(botLimit, topLimit));

        TMP_Text killText = pooledText.GetComponent<TMP_Text>();
        killText.text = $"{killedPlayerName} {_feedEndText}";

        Sequence tweenSequence = DOTween.Sequence();

        tweenSequence.Insert(0, killText.transform.DOPunchScale(Vector3.one * _textscalePunchValue, _textScalePunchDuration))
            .Insert(0, killText.transform.DOPunchRotation(new Vector3(0, 0, 50), _textScalePunchDuration, 10, 5))
            .AppendInterval(_textLifeTime)
            .Append(killText.DOFade(0, .5f));

        tweenSequence.onComplete += Reset;
        tweenSequence.Play();

        void Reset()
        {
            killText.alpha = 1;
            LocalPoolManager.Instance.Pool_UI_Kill.PutObjectBackInPool(pooledText);
            _textCount--;
        }
    }

}
