using System;
using UnityEngine;
using UnityEngine.VFX;


public class Cons_Bomb : Cons_ThrowableBase
{
    [SerializeField] GameObject _bombPrefab;

    [SerializeField] float _fuseDuration;

    [SerializeField]
    private VisualEffect _fuseVFX;
    float _fuseValue;

    private bool _isPlayingThrowAnimation = false;

    public override void Equip()
    {
        base.Equip();
        _fuseVFX.Stop();
    }

    public override void StartUsing()
    {
        base.StartUsing();

        _fuseValue = 0;
        _fuseVFX.Play();
    }

    //tant qu'on reste appuyé
    public override void UseUpdate()
    {
        base.UseUpdate();

        if (_throwAnimationIsPlaying) return; 
        
        //incrementation du temps de fuse
        _fuseValue += Time.deltaTime;

        if (_fuseValue >= _fuseDuration)
            Explode();
    }

    //quand on relache le bouton
    public override void StopUsing()
    {
        base.StopUsing();
        
        //lancer la bombe
        StartThrowAnimation();
    }

    protected override void ThrowObject()
    {
        //spawn du projectile
        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData = _fuseValue;
        context.floatData2 = ItemID;
        Summoner.Instance.SpawnObject(_bombPrefab, hand._itemSocket.position, hand.equippedItem.playerCharacter.playerCamera.transform.rotation, false, context);
        _fuseValue = 0;
    }

    void Explode()
    {
        //todo => explose dans tes mains - fait spawn une explosion sur le joueur
        ThrowObject();
        BreakItem();
    }
}
