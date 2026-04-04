using UnityEngine;

public class Crosshair<T> : MonoBehaviour where T : Item
{
    [Header("References")]
    [SerializeField] protected GameObject visuals;
    [SerializeField] protected CrosshairsManager manager;

    protected T weapon;

    public virtual void Activate(T weapon)
    {
        visuals.SetActive(true);
        this.weapon = weapon;

        manager.OnActivateCrosshair += Deactivate;
    }

    protected virtual void Deactivate()
    {
        visuals?.SetActive(false);
        weapon = null;

        manager.OnActivateCrosshair -= Deactivate;
    }
}
