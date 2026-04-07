using UnityEngine;
using UnityEngine.UI;

public class HammerCrosshair : Crosshair<Hammer>
{
    [Header("Hammer Crosshair References")]
    [SerializeField] GameObject _dashCH;
    [SerializeField] Slider _cooldownSlider;

    public override void Activate(Hammer weapon)
    {
        base.Activate(weapon);

        weapon.OnStartCharging += SwapCrosshairState;
        weapon.OnStopCharging += SwapCrosshairState;
    }

    void SwapCrosshairState()
    {
        _dashCH.SetActive(!_dashCH.activeSelf);
    }

    private void Update()
    {
        _cooldownSlider.value = weapon.currentDashCooldown / weapon.dashCooldown;
    }

}
