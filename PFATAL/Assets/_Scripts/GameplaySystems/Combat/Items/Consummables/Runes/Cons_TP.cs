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

        _playerCharacter.physics.SetPosition(tpDestination);
        
        HideMarker();
        BreakItem();
    }

    public override void UseUpdate()
    {
        base.UseUpdate();

        RaycastHit hit;

        if(Physics.Raycast(_playerCharacter.playerCamera.transform.position, _playerCharacter.playerCamera.transform.forward,out hit, _range, _layermask))
        {
            tpDestination = hit.point - _playerCharacter.playerCamera.transform.forward * _wallOffsetRange;
        }
        else
            tpDestination = _playerCharacter.playerCamera.transform.position + _playerCharacter.playerCamera.transform.forward * _range;

        marker.transform.position = tpDestination;
    }

    public override void OnUnEquip()
    {
        base.OnUnEquip();

        HideMarker();
    }

    void HideMarker()
    {
        Destroy(marker);
    }
}
