using System;
using UnityEngine;
using UnityEngine.VFX;


public class Cons_Bomb : Cons_ThrowableBase
{
    private static readonly int Tint = Shader.PropertyToID("_tint");
    private static readonly int Exposure = Shader.PropertyToID("_exposure");


    [SerializeField] GameObject _bombPrefab;
    [SerializeField] float _fuseDuration;
    
    [Header("Scene References")]
    [SerializeField] private VisualEffect _fuseVFX;

    [SerializeField] private Renderer _bombRenderer;
    
    //fuse animation
    float _fuseValue;
    private float _fuseIgniteTime;
    MaterialPropertyBlock _bombPropertyBlock;
    
    private bool _isPlayingThrowAnimation = false;

    public override void Equip()
    {
        base.Equip();

        _fuseIgniteTime = 0;

        //reset visuals
        _fuseVFX.Stop();
        _bombRenderer.enabled = true;
        _bombPropertyBlock??=new MaterialPropertyBlock();
        _bombPropertyBlock.SetColor(Tint, new Color(1, 1, 1, 0));
        _bombPropertyBlock.SetFloat(Exposure,1);
        _bombRenderer.SetPropertyBlock(_bombPropertyBlock);
        _bombRenderer.transform.localScale = Vector3.one*100;
    }

    public override void StartUsing()
    {
        if (_throwAnimationIsPlaying) return;

        base.StartUsing();

        _fuseValue = 0;
        _fuseIgniteTime = Time.time;
        _fuseVFX.Play();
    }

    void Update()
    {
        //faire clignoter la bombe
        if ((isUsing || _throwAnimationIsPlaying) && _fuseValue>0)
        {
            float alpha = 1.0f-Mathf.Abs(Mathf.Cos(_fuseValue/(_fuseDuration*_fuseDuration)*(Time.time-_fuseIgniteTime)*5*2*Mathf.PI));
            
            _bombPropertyBlock.SetColor(Tint,Color.Lerp(
                new Color(1, 1, 1, 0),
                new Color(2,.5f,.5f),
                alpha));
            _bombPropertyBlock.SetFloat(Exposure,1+alpha);
            _bombRenderer.SetPropertyBlock(_bombPropertyBlock);
            
            _bombRenderer.transform.localScale = Vector3.one * (100+alpha*8);
            
        }
            
    }

    //tant qu'on reste appuyé
    public override void UseUpdate()
    {
        base.UseUpdate();

        if (_throwAnimationIsPlaying) return; 
        
        //incrementation du temps de fuse
        _fuseValue += Time.deltaTime;
        
        //explosion quand on cook la bombe trop longtemps
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
        _bombRenderer.enabled = false;
        _fuseVFX.Stop();
        
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
        _fuseValue = _fuseDuration;

        ThrowObject();
        BreakItem();
    }
}
