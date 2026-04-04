using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class TomahawkCrosshair : Crosshair<Tomahawk>
{
    [Header("Direction Indicator Settings")]
    [SerializeField] DirectionIndicator _indicator;

    GameObject _tomahawkProj;

    public override void Activate(Tomahawk tomahawk)
    {
        base.Activate(tomahawk);

        weapon.OnTomahawkShoot += SetProj;
    }

    void SetProj(GameObject proj)
    {
        _tomahawkProj = proj;
        _indicator.gameObject.SetActive(true);
        _indicator.Setup(_tomahawkProj.transform.position,manager.hud.playerCharacter.transform);
    }

    private void Update()
    {
        if(_tomahawkProj != null)
        {
            _indicator.SetTargetpos(_tomahawkProj.transform.position);
        }
        else if(_indicator.gameObject.activeSelf)
        {
            _indicator.gameObject.SetActive(false);
        }
    }
}
