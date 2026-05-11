using UnityEngine;

public class CrossbowCrosshair : Crosshair<Crossbow>
{
    [Header("Crossbow References")]
    [SerializeField] RectTransform _zoomingCrosshair;

    [Header("Crossbow Settings")]
    [SerializeField] float _margin = 10;
    [SerializeField] float _resetTimeMultiplyer = 5;

    private void Update()
    {
        float size = 100;
        if(weapon.normalizedChargeValue > 0)
        {
            size = 100 - weapon.normalizedChargeValue * (100 - _margin);
           
        }
        else if(_zoomingCrosshair.sizeDelta.x < 100)
        {
            size = Mathf.Lerp(_zoomingCrosshair.sizeDelta.x, 100, Time.deltaTime * _resetTimeMultiplyer);
        }
        _zoomingCrosshair.sizeDelta = new Vector2(size, size);
    }
}
