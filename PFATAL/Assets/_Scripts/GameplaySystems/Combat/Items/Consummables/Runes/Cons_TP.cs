using UnityEngine;

public class Cons_TP : Consummable
{
    [SerializeField] float _range;
    [SerializeField] float _wallOffsetRange = .5f;
    [SerializeField] GameObject _markerPrefab;
    [SerializeField] LayerMask _layermask;

    GameObject marker;
    Vector3 tpDestination;

    public override void StartUsing()
    {
        marker = Instantiate(_markerPrefab, transform);

        base.StartUsing();
    }

    public override void StopUsing()
    {
        base.StopUsing();

        playerCharacter.physics.SetPosition(tpDestination);
        
        HideMarker();
        BreakItem();
    }

    public override void UseUpdate()
    {
        base.UseUpdate();

        RaycastHit hit;

        if(Physics.Raycast(playerCharacter.playerCamera.transform.position, playerCharacter.playerCamera.transform.forward,out hit, _range, _layermask))
        {
            tpDestination = hit.point - playerCharacter.playerCamera.transform.forward * _wallOffsetRange;
        }
        else
            tpDestination = playerCharacter.playerCamera.transform.position + playerCharacter.playerCamera.transform.forward * _range;

        marker.transform.position = tpDestination;
    }

    public override void UnEquip()
    {
        base.UnEquip();

        HideMarker();
    }

    void HideMarker()
    {
        Destroy(marker);
    }
}
