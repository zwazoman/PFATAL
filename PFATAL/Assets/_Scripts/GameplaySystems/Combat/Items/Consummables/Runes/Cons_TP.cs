using System;
using GameplaySystems.PlayerCharacter;
using UnityEngine;

public class Cons_TP : Consummable
{
    [SerializeField] float _range;
    [SerializeField] float _wallOffsetRange = .5f;
    [SerializeField] GameObject _markerPrefab;
    [SerializeField] LayerMask _layermask;

    GameObject marker;
    Vector3 tpDestination;

    private bool animationIsPlaying = false;

    public override void Equip()
    {
        base.Equip();
        animationIsPlaying = false;
        hand.animatorEventListener.OnGemBroken += BreakGem;
    }

    //quand on appuie sur la touche
    public override void StartUsing()
    {
        if (animationIsPlaying)return;
        
        //afficher le marker de TP
        marker = Instantiate(_markerPrefab);
        base.StartUsing();
    }

    //quand on relache la touche
    public override void StopUsing()
    {
        if (animationIsPlaying) return;
        
        //lancer l'animation de break
        animationIsPlaying = true;
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.gem_break);
        base.StopUsing();
    }

    //appelée par l'animation event
    void BreakGem()
    {
        playerCharacter.physics.SetPosition(tpDestination);
        //hand.animatorEventListener.OnGemBroken -= BreakGem;
        HideMarker();
        BreakItem();
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

    public override void UnEquip()
    {
        base.UnEquip();
        HideMarker();
        hand.animatorEventListener.OnGemBroken -= BreakGem;
    }

    void HideMarker()
    {
        Destroy(marker);
        marker = null;
    }
}
