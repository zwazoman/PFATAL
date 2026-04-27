using UnityEngine;
using UnityEngine.UI;

public class HammerCrosshair : Crosshair<Sword>
{
    [Header("Hammer Crosshair References")]
    [SerializeField] GameObject _dashCH;
    [SerializeField] Slider _cooldownSlider;

    public override void Activate(Sword weapon)
    {
        base.Activate(weapon);

        weapon.OnStartCharging += SwapCrosshairState;
        weapon.OnStopCharging += SwapCrosshairState;
    }

    protected override void Deactivate()
    {
        weapon.OnStartCharging -= SwapCrosshairState;
        weapon.OnStopCharging -= SwapCrosshairState;

        _dashCH.SetActive(false);

        base.Deactivate();
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
