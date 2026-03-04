using UnityEngine;

public class Cons_TP : Consummable
{
    [SerializeField] float range;

    [SerializeField] GameObject markerPrefab;

    GameObject marker;
    Vector3 tpDestination;

    public override void StartUsing()
    {
        base.StartUsing();

        tpDestination = main.playerCamera.transform.position + main.playerCamera.transform.forward * range;

        ShowMarker();
    }

    public override void StopUsing()
    {
        base.StopUsing();

        main.transform.position = tpDestination;
        
        HideMarker();
        BreakItem();
    }

    public override void OnUnEquip()
    {
        base.OnUnEquip();

        HideMarker();
    }

    void ShowMarker()
    {
        marker = Instantiate(markerPrefab, transform);
        marker.transform.position = tpDestination;
    }

    void HideMarker()
    {
        Destroy(marker);
    }
}
