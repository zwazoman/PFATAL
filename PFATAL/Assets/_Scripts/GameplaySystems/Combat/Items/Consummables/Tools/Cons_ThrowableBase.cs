using DG.Tweening;
using GameplaySystems.PlayerCharacter;
using System;
using UnityEngine;

/// <summary>
/// classe de base qui gere l'activation des consommable lançables selon l'animation.
/// Les classes enfant doivent appeler "StartThrowAnimation" et override "ThrowObject".
/// </summary>
public abstract class Cons_ThrowableBase : Consummable
{
    [Header("References")]
    [SerializeField] public Transform shootSocket;

    [Header("Projectile Settings")]
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected GameObject visualProjectile;
    [SerializeField] protected ProjectileRotationMode rotationMode = ProjectileRotationMode.Camera;

    [SerializeField] protected LayerMask shootRayLayerMask;

    protected Projectile _currentProjectile;

    protected bool _throwAnimationIsPlaying { get; private set; } = false;

    public enum ProjectileRotationMode
    {
        Camera,   // vise là où la caméra regarde
        Player,   // transform.forward du joueur
    }

    public override void Equip()
    {
        base.Equip();
        _throwAnimationIsPlaying = false;
        
        //link events
        hand.animatorEventListener.OnObjectThrown += OnObjectThrown;
        hand.animatorEventListener.OnAnimationFinished += OnAnimationFinished;
    }

    public override void UnEquip()
    {
        //unlink events
        hand.animatorEventListener.OnObjectThrown -= OnObjectThrown;
        hand.animatorEventListener.OnAnimationFinished -= OnAnimationFinished;
        
        base.UnEquip();
    }

    protected void StartThrowAnimation()
    {
        if (_throwAnimationIsPlaying) return;
        
        _throwAnimationIsPlaying = true;
        hand.visuals.PlayAnimation(PlayerHandVisuals.AnimationID.bomb_use);
    }
    
    /// <summary>
    /// appelé par l'animation, à ovveride.
    /// </summary>
    protected abstract void ThrowObject();
    
    //animation callbacks
    private void OnAnimationFinished()
    {
        if (_throwAnimationIsPlaying)
        {
            _throwAnimationIsPlaying = false;
            BreakItem();
        }
    }
    void OnObjectThrown()
    {
        ThrowObject();
    }

    protected virtual async Awaitable<GameObject> SpawnObject(SpawnContext spawnContext, Quaternion rotationOffset, Vector3 spawnPos)
    {
        Proj_Visual visual = null;

        Vector3 mirorPos;
        Vector3 newPos = playerCharacter.playerCamera.ScreenToWorldPoint(new Vector3(playerCharacter.handsCamera.WorldToScreenPoint(shootSocket.position).x, playerCharacter.handsCamera.WorldToScreenPoint(shootSocket.position).y, .3f));
        mirorPos = newPos;

        //fait spawn un projectile "miroir" imitant les déplacements du vrai projectile sans délai chez le client
        if (visualProjectile != null)
        {
            Instantiate(visualProjectile, mirorPos, ComputeProjectileRotation(mirorPos) * rotationOffset).TryGetComponent(out visual);
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
