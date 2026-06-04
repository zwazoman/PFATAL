using System;
using DG.Tweening;
using GameplaySystems.PlayerCharacter;
using UnityEngine;

public abstract class Cons_BookBase : Consummable
{
    private static readonly int Active_AnimProperty = Animator.StringToHash("Active");
    private static readonly int Charge_AnimProperty = Animator.StringToHash("Charge");

    protected bool _spellAnimationIsPlaying { get; private set; } = false;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _Vfx;
    private Vector3 _vfxBaseScale;

    [Header("References")]
    [SerializeField] public Transform shootSocket;

    [Header("Projectile Settings")]
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected GameObject visualProjectile;
    [SerializeField] protected ProjectileRotationMode rotationMode = ProjectileRotationMode.Camera;

    [SerializeField] protected LayerMask shootRayLayerMask;

    protected Projectile _currentProjectile;

    public enum ProjectileRotationMode
    {
        Camera,   // vise là où la caméra regarde
        Player,   // transform.forward du joueur
    }

    protected virtual void Awake()
    {
        _vfxBaseScale = _Vfx.localScale;
    }

    public override void Equip()
    {
        base.Equip();
        _spellAnimationIsPlaying = false;
        hand.animatorEventListener.OnSpellCast += OnSpellCast;
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;

        //reset anim
        _animator.SetBool(Active_AnimProperty, false);
        _Vfx.localScale = _vfxBaseScale;
    }

    public override void UnEquip()
    {
        //reset anim
        _animator.SetBool(Active_AnimProperty, false);
        _Vfx.localScale = _vfxBaseScale;

        //unlink events
        hand.animatorEventListener.OnSpellCast -= OnSpellCast;
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;

        base.UnEquip();
    }

    protected void StartChargeIdle()
    {
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.book_charge_idle);
        _animator.SetTrigger(Charge_AnimProperty);
    }

    protected void StartCastAnimation()
    {
        _spellAnimationIsPlaying = true;
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.book_use);
        _animator.SetBool(Active_AnimProperty, true);
        _Vfx.DOScale(0, .5f).SetEase(Ease.OutQuad);

    }

    public override void StartUsing()
    {
        base.StartUsing();
        if (!_spellAnimationIsPlaying)
        {
            StartChargeIdle();
            StopScrollable();
        }
        
    }

    public override void StopUsing()
    {
        base.StopUsing();
        if (!_spellAnimationIsPlaying)
            StartCastAnimation();
    }

    /// <summary>
    /// appelé par l'animation, à ovveride.
    /// </summary>
    protected abstract void ApplySpellEffect();

    //animation callbacks
    private void OnAnimationFinished()
    {
        if (_spellAnimationIsPlaying)
        {
            _spellAnimationIsPlaying = false;
            Scrollable();
            BreakItem();
        }
    }

    void OnSpellCast()
    {
        ApplySpellEffect();
    }

    protected virtual async Awaitable<GameObject> SpawnSpell(SpawnContext spawnContext, Quaternion rotationOffset, Vector3 spawnPos)
    {
        Proj_Visual visual = null;

        Vector3 mirorPos;
        Vector3 newPos = playerCharacter.playerCamera.ScreenToWorldPoint(new Vector3(playerCharacter.handsCamera.WorldToScreenPoint(shootSocket.position).x, playerCharacter.handsCamera.WorldToScreenPoint(shootSocket.position).y, .3f));
        mirorPos = newPos;

        if (visualProjectile != null)
        {
            Instantiate(visualProjectile, mirorPos, ComputeProjectileRotation(mirorPos) * rotationOffset).TryGetComponent(out visual); ;
            visual.context = spawnContext;
        }

        GameObject _currentProjectileObject = await Summoner.Instance.SpawnObject(projectile, spawnPos, ComputeProjectileRotation(spawnPos) * rotationOffset, true, spawnContext);
        _currentProjectile = _currentProjectileObject.GetComponent<Projectile>();

        if (visual != null)
            visual.trueProjectile = _currentProjectile;

        return _currentProjectileObject;
    }

    protected Quaternion ComputeProjectileRotation(Vector3 spawnPos)
    {
        if (rotationMode == ProjectileRotationMode.Player)
            return playerCharacter.transform.rotation;

        Quaternion rotation;

        RaycastHit hit;
        if (playerCharacter.inputs.UsingGamePad == true)
        {
            if (Physics.SphereCast(playerCharacter.playerCamera.transform.position, 1f, playerCharacter.playerCamera.transform.forward, out hit, 100f, LayerMask.GetMask("Player")))
            {
                //Vector3 direction = shootSocket.position - hit.point;
                Vector3 direction = (hit.point - spawnPos).normalized;

                //rotation = Quaternion.LookRotation(-direction, transform.up);
                rotation = Quaternion.LookRotation(direction);
                return rotation;
            }
            else
            {
                rotation = playerCharacter.playerCamera.transform.rotation;//shootSocket.rotation;
                return rotation;
            }
        }
        else
        {
            if (Physics.Raycast(playerCharacter.playerCamera.transform.position, playerCharacter.playerCamera.transform.forward, out hit, Mathf.Infinity, shootRayLayerMask))
            {
                Vector3 direction = hit.point - spawnPos;
                rotation = Quaternion.LookRotation(direction, Vector3.up);
                return rotation;
            }
            else
            {
                rotation = playerCharacter.playerCamera.transform.rotation;
                return rotation;
            }
        }
    }
}