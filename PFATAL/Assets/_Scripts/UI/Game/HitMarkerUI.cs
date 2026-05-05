using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HitMarkerUI : MonoBehaviour
{
    public event Action OnShowHitMarker;

    [Header("References")]
    [SerializeField] HUDManager _hud;
    [SerializeField] Image _hitmarkerImage;

    [Header("Settings")]
    [SerializeField] float _duration = .3f;
    [SerializeField] float _scalePunchIntensity = 3;
    [SerializeField] float _rotationOffset = 20;
    [SerializeField] Color32 _baseColor, _killColor;

    Vector3 _initialScale;

    bool _playing = false;

    private void Start()
    {
        _hud.OnTriggerHitFeedback += ShowHitMarker;
        _initialScale = _hitmarkerImage.transform.localScale;
        _hitmarkerImage.color = _baseColor;
    }

    public void ShowHitMarker(bool killed)
    {
        if (_playing)
            return;

        _playing = true;

        OnShowHitMarker?.Invoke();

        if (killed)
            _hitmarkerImage.color = _killColor;

        _hitmarkerImage.gameObject.SetActive(true);
        _hitmarkerImage.transform.localScale = _initialScale;


        _hitmarkerImage.transform.DOPunchRotation(new Vector3(0, 0, Random.Range(-_rotationOffset, _rotationOffset)), _duration, 10, 10);
        _hitmarkerImage.transform.DOPunchScale(Vector2.one * _scalePunchIntensity, _duration).onComplete += OnPunchScale_Callback;
    }

    void OnPunchScale_Callback()
    {
        _playing = false;
        _hitmarkerImage.gameObject.SetActive(false);
        _hitmarkerImage.color = _baseColor;
    }

}
