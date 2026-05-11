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

    private bool breakAnimationIsPlaying = false;

    public override void Equip()
    {
        base.Equip();
        breakAnimationIsPlaying = false;
        hand.animatorEventListener.OnGemBroken += BreakGem;
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;
    }

    //quand on appuie sur la touche
    public override void StartUsing()
    {
        if (breakAnimationIsPlaying)return;
        
        //afficher le marker de TP
        marker = Instantiate(_markerPrefab);
        base.StartUsing();
    }

    //quand on relache la touche
    public override void StopUsing()
    {
        if (breakAnimationIsPlaying) return;
        
        //lancer l'animation de break
        breakAnimationIsPlaying = true;
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.gem_break);
        base.StopUsing();
    }

    //appelée par un animation event
    void BreakGem()
    {
        playerCharacter.physics.SetPosition(tpDestination);
        //hand.animatorEventListener.OnGemBroken -= BreakGem;
        HideMarker();
    }

    //appelé par un animation event
    void OnAnimationFinished()
    {
        if (breakAnimationIsPlaying)
        {
            breakAnimationIsPlaying = false;
            BreakItem();
        }
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
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;
    }

    void HideMarker()
    {
        Destroy(marker);
        marker = null;
    }
}
