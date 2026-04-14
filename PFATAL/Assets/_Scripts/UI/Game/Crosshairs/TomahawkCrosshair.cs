using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TomahawkCrosshair : Crosshair<Tomahawk>
{
    [Header("Crosshair settings")]
    [SerializeField] DirectionIndicator _indicator;
    [SerializeField] Slider _dashCooldoawnSlider;
    [SerializeField] List<GameObject> _ammunitions;

    GameObject _tomahawkProj;

    public override void Activate(Tomahawk tomahawk)
    {
        base.Activate(tomahawk);

        weapon.OnTomahawkShoot += SetProj;

        weapon.OnConsumeAmmo += SetAmmos;
        weapon.OnLoadAmmo += SetAmmos;

        SetAmmos(weapon.maxAmmoAmount);
    }

    protected override void Deactivate()
    {
        weapon.OnTomahawkShoot -= SetProj;

        weapon.OnConsumeAmmo -= SetAmmos;
        weapon.OnLoadAmmo -= SetAmmos;

        base.Deactivate();
    }

    void SetAmmos(int newAmount)
    {
        foreach(GameObject ammo in _ammunitions)
            ammo.SetActive(false);

        if (newAmount > 0)
        {
            for (int i = 0; i < newAmount; i++)
            {
                _ammunitions[i].SetActive(true);
            }
        }
    }

    void SetProj(Projectile proj)
    {
        _tomahawkProj = proj.gameObject;
        _indicator.gameObject.SetActive(true);
        _indicator.Setup(_tomahawkProj.transform.position, manager.hud.playerCharacter);
    }

    private void Update()
    {
        if (_tomahawkProj != null)
        {
            _indicator.SetTargetpos(_tomahawkProj.transform.position);
        }
        else if (_indicator.gameObject.activeSelf)
        {
            _indicator.gameObject.SetActive(false);
        }

        _dashCooldoawnSlider.value = weapon.currentDashCooldown / weapon.dashCooldown;
    }
}
