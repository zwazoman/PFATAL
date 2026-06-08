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
        print("ahhh");
        _textCount++;

        PooledObject pooledText = LocalPoolManager.Instance.Pool_UI_Kill.PullObjectFromPool(transform);

        //todo => faut g�rer la position en fonction du nombre. placer en dessous en fonction du nombre affich�s
        //layout group juste ? prendre des refs
        
        //pooledText.transform.localPosition = new Vector2(Random.Range(leftLimit, rightLimit), Random.Range(botLimit, topLimit));

        pooledText.transform.GetChild(0).TryGetComponent(out TMP_Text killText);
        pooledText.transform.TryGetComponent(out CanvasGroup canvaGroup);
        pooledText.transform.localPosition = Vector3.zero;
        killText.text = $"Slayed <color=#FB0D4A>{killedPlayerName}</color> !";
        
        Sequence tweenSequence = DOTween.Sequence();
        
        tweenSequence.Insert(0,
                pooledText.transform.DOPunchScale(Vector3.one * _textscalePunchValue, _textScalePunchDuration,5,.5f))
            .Insert(0, 
                pooledText.transform.DOPunchRotation(new Vector3(0, 0, 5), _textScalePunchDuration, 5, 5))
            .AppendInterval(_textLifeTime)
            .Append(canvaGroup.DOFade(0, .3f).SetEase(Ease.OutQuad));

        tweenSequence.onComplete += Reset;
        tweenSequence.Play();

        void Reset()
        {
            canvaGroup.alpha = 1;
            LocalPoolManager.Instance.Pool_UI_Kill.PutObjectBackInPool(pooledText);
            _textCount--;
        }
    }

}
