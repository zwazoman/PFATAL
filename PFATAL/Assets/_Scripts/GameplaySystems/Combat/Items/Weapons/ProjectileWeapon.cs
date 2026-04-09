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

    protected bool canShoot = true;

    /// <summary>
    /// g�re le delay entre 2 tirs
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
    protected virtual async Awaitable<GameObject> Shoot(SpawnContext spawnContext, Quaternion rotation)
    {
        OnShoot?.Invoke();
        StartShootDelay();

        //todo => faire spawn un faux projectile coté client qui part du socket de l'arme et follow la trajectoire du vrai projo
        if(visualProjectile != null)
        {
            Instantiate(visualProjectile, shootSocket.position, ComputeProjectileRotation(shootSocket.position)).TryGetComponent(out Proj_Visual visual);
            visual.context = spawnContext;
        }

        return await Summoner.Instance.SpawnObject(projectile, playerCharacter.playerCamera.transform.position +Vector3.down * shootSocketDownPosMult, rotation,true, spawnContext);
    }

    protected Quaternion ComputeProjectileRotation(Vector3 spawnPos)
    {
        Quaternion rotation;
        
        RaycastHit hit;
        if (playerCharacter.inputs.UsingGamePad == true)
        {
            if (Physics.SphereCast(playerCharacter.playerCamera.transform.position, 1f,playerCharacter.playerCamera.transform.forward, out hit, 100f, LayerMask.GetMask("Player")))
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
                rotation = Quaternion.LookRotation(direction, transform.up);
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
