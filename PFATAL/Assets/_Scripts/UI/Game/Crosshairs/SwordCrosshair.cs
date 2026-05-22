using UnityEngine;
using UnityEngine.UI;

public class SwordCrosshair : Crosshair<Sword>
{
    [Header("Hammer Crosshair References")]
    [SerializeField] GameObject _dashCH;
    [SerializeField] Slider _cooldownSlider;

    public override void Activate(Sword weapon)
    {
        print(weapon);

        base.Activate(weapon);

        weapon.OnStartCharging += ActivateDashCrosshair;
        weapon.OnAttackEnded += DeactivateDashCrosshair;
    }

    protected override void Deactivate()
    {
        weapon.OnStartCharging -= ActivateDashCrosshair;
        weapon.OnAttackEnded -= DeactivateDashCrosshair;

        _dashCH.SetActive(false);

        base.Deactivate();
    }

    void ActivateDashCrosshair() => _dashCH.SetActive(true);

    void DeactivateDashCrosshair() => _dashCH.SetActive(false);


    private void Update()
    {
        _cooldownSlider.value = weapon.currentDashCooldown / weapon.dashCooldown;
    }

}
