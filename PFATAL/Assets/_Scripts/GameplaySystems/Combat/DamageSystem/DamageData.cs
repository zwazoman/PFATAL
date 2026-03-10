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
    /// Le centre de l’explosion / l'endroit où l'ennemi a été touché
    /// </summary>
    public Vector3 Point;

    /// <summary>
    /// le forward de la source des dégats
    /// </summary>
    public Vector3 Direction;
    
    //degats de zone
    
    /// <summary>
    /// Le rayon de l’explosion
    /// </summary>
    public float Radius;

    public DamageData(float amount, Vector3 point, Vector3 direction, ulong sourcePlayerClientID = NON_PLAYER_DAMAGE_SOURCE_CLIENT_ID, float radius = 0)
    {
        Amount = amount;
        SourcePlayerClientID = sourcePlayerClientID;
        Point = point;
        Direction = direction;
        Radius = radius;
    }
}
