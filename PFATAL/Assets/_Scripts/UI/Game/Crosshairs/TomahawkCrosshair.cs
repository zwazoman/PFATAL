using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TomahawkCrosshair : Crosshair<Tomahawk>
{
    [Header("Crosshair settings")]
    [SerializeField] DirectionIndicator _indicator;
    [SerializeField] List<GameObject> _ammunitions;

    GameObject _tomahawkProj;

    public override void Activate(Tomahawk tomahawk)
    {
        base.Activate(tomahawk);

        weapon.OnTomahawkShoot += SetProj;

        weapon.OnShoot += SetAmmos;
        weapon.OnLoadAmmo += SetAmmos;

        SetAmmos();
    }

    protected override void Deactivate()
    {
        base.Deactivate();

        weapon.OnTomahawkShoot -= SetProj;

        weapon.OnShoot -= SetAmmos;
        weapon.OnLoadAmmo -= SetAmmos;
    }

    void SetAmmos()
    {
        foreach(GameObject ammo in _ammunitions)
            ammo.SetActive(false);

        if (weapon.currentAmmoCount > 0)
        {
            for (int i = 0; i < weapon.currentAmmoCount; i++)
            {
                print(i);
                _ammunitions[i].SetActive(true);
            }
        }
    }

    void SetProj(GameObject proj)
    {
        _tomahawkProj = proj;
        _indicator.gameObject.SetActive(true);
        _indicator.Setup(_tomahawkProj.transform.position, manager.hud.playerCharacter.transform);
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
    }
}
