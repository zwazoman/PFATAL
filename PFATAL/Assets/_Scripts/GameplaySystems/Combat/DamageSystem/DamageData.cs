using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Représente toutes les infos liées à un dégat
/// </summary>
public struct DamageData : INetworkSerializeByMemcpy
{
    public const ulong NON_PLAYER_DAMAGE_SOURCE_CLIENT_ID = 1000;
    
    /// <summary>
    /// La quantité de dégats à appliquer
    /// </summary>
    public float Amount;
    
    /// <summary>
    /// Le client ID du joueur qui a provoqué les dégats. NON_PLAYER_DAMAGE_SOURCE_CLIENT_ID -> dégats pas provoqués par un joueur (piège...)
    /// </summary>
    public ulong SourcePlayerClientID;
    
    /// <summary>
    /// le centre de l'explosion /l'endroit où l'ennemi a été touché
    /// </summary>
    public Vector3 Point;

    /// <summary>
    /// la position du joueur qui a tiré/de la source des dégâts
    /// </summary>
    public Vector3 SourcePos;

    /// <summary>
    /// le forward de la source des dégats
    /// </summary>
    public Vector3 Direction;

    /// <summary>
    /// la force du knockback appliqué à la cible si applicable
    /// </summary>
    public Vector3 KnockbackForce;
    
    //degats de zone
    
    /// <summary>
    /// Le rayon de l’explosion
    /// </summary>
    public float Radius;

    /// <summary>
    /// L'ID de l'arme utilisée pour infliger les dégâts.
    /// </summary>
    public int WeaponID;

    public DamageData(float amount, Vector3 point, Vector3 sourcePos, Vector3 direction, ulong sourcePlayerClientID = NON_PLAYER_DAMAGE_SOURCE_CLIENT_ID, float radius = 0, int weaponID = 0)
    {
        Amount = amount;
        SourcePlayerClientID = sourcePlayerClientID;
        Point = point;
        SourcePos = sourcePos;
        KnockbackForce = Vector3.zero;
        Direction = direction;
        Radius = radius;
        WeaponID = weaponID;
    }
}
