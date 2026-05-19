using System;
using GameplaySystems.PlayerCharacter;
using UnityEngine;

public class Cons_TP : Cons_RuneBase
{
    [SerializeField] float _range;
    [SerializeField] float _wallOffsetRange = .5f;
    [SerializeField] GameObject _markerPrefab;
    [SerializeField] LayerMask _layermask;

    GameObject marker;
    Vector3 tpDestination;
    

    //quand on appuie sur la touche
    public override void StartUsing()
    {
        if (_breakAnimationIsPlaying) return;
        
        //afficher le marker de TP
        marker = Instantiate(_markerPrefab);
        base.StartUsing();
    }

    //quand on relache la touche
    public override void StopUsing()
    {
        if (_breakAnimationIsPlaying) return;
        
        //lancer l'animation de break
        StartBreakingAnimation();
        base.StopUsing();
    }
    
    public override void UnEquip()
    {
        base.UnEquip();
        HideMarker();
    }
    
    //appelée par un animation event
    protected override void ApplyGemEffect()
    {
        playerCharacter.physics.SetPosition(tpDestination);
        HideMarker();
    }
    protected virtual void Update()
    {
        //Update TP marker position
        if (marker != null)
        {
            RaycastHit hit;

            if(Physics.SphereCast(playerCharacter.playerCamera.transform.position,.3f, playerCharacter.playerCamera.transform.forward,out hit, _range, _layermask))
            {
                tpDestination = hit.point - playerCharacter.playerCamera.transform.forward * _wallOffsetRange;
            }
            else
                tpDestination = playerCharacter.playerCamera.transform.position + playerCharacter.playerCamera.transform.forward * _range;

            marker.transform.position = tpDestination;
        }
        
    }

    //cache le marker de previsualisation du tp
    void HideMarker()
    {
        Destroy(marker);
        marker = null;
    }
}
