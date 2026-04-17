using NetworkTime;
using System;
using UnityEngine;

public class ProjectileWeapon : Item
{
    public event Action OnShoot;
    public event Action OnShootDelayEnd;

    [Header("References")]
    [SerializeField] public Transform shootSocket;

    [Header("Weapon Settings")]
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected GameObject visualProjectile;
    [SerializeField] float shootDelay;

    [SerializeField] protected float shootSocketDownPosMult = .1f;

    [SerializeField] protected LayerMask shootRayLayerMask;

    protected Projectile _currentProjectile;
    protected bool canShoot = true;

    public override void UnEquip()
    {
        base.UnEquip();

        _currentProjectile = null;
    }

    /// <summary>
    /// gere le delay entre 2 tirs
    /// </summary>
    protected async void StartShootDelay()
    {
        canShoot = false;

        await Awaitable.WaitForSecondsAsync(shootDelay);

        OnShootDelayEnd?.Invoke();
        canShoot = true;
    }

    /// <summary>
    /// prend en param�tre un context, spawn le projectile donn� et le tourne vers le point d'un raycast tir� depuis la cam�ra
    /// </summary>
    /// <param name="spawnContext"> le context du spawn</param>
    protected virtual async Awaitable<GameObject> Shoot(SpawnContext spawnContext, Quaternion rotationOffset, Vector3 spawnPos)
    {
        OnShoot?.Invoke();
        StartShootDelay();

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

        print(TimeStamp.Now);

        GameObject _currentProjectileObject = await Summoner.Instance.SpawnObject(projectile, spawnPos, ComputeProjectileRotation(spawnPos) * rotationOffset, true, spawnContext);
        _currentProjectile = _currentProjectileObject.GetComponent<Projectile>();

        if (visual != null)
            visual.trueProjectile = _currentProjectile;

        return _currentProjectileObject;
    }

    protected Quaternion ComputeProjectileRotation(Vector3 spawnPos)
    {
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
