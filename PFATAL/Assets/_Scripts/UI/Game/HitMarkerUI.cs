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

    private void Start()
    {
        _hud.OnTriggerHitFeedback += ShowHitMarker;
    }

    public void ShowHitMarker()
    {
        OnShowHitMarker?.Invoke();

        _hitmarkerImage.gameObject.SetActive(true);

        //_hitmarkerImage.transform.eulerAngles = Vector3.zero;
        //_hitmarkerImage.transform.eulerAngles = new Vector3(0, 0, Random.Range(-_rotationOffset, _rotationOffset));

        _hitmarkerImage.transform.DOPunchRotation(new Vector3(0, 0, Random.Range(-_rotationOffset, _rotationOffset)), _duration);
        _hitmarkerImage.transform.DOPunchScale(Vector2.one * _scalePunchIntensity, _duration).onComplete += OnPunchScale_Callback;
    }

    void OnPunchScale_Callback()
    {
        _hitmarkerImage.gameObject.SetActive(false);
    }

}
