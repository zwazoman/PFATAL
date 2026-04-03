using System;
using UnityEngine;
using UnityEngine.UI;

public class HitMarkerUI : MonoBehaviour
{
    public event Action OnShowHitMarker;

    [Header("References")]
    [SerializeField] Image _hitmarkerImage;

    [Header("Settings")]
    [SerializeField] float _duration;

    public void ShowHitMarker()
    {
        OnShowHitMarker?.Invoke();
    }
}
