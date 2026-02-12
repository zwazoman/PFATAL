using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Représente toutes les infos liées à un dégat
/// </summary>
/// <typeparam name="T">La source du dégat (joueur, piège, killzone...) </typeparam>
public struct DamageData : INetworkSerializeByMemcpy
{
    /// <summary>
    /// La quantité de dégats à appliquer
    /// </summary>
    public float Amount;
    
    /// <summary>
    /// Le client ID du joueur qui a provoqué les dégats. -1 -> dégats pas provoqués par un joueur (piège...)
    /// </summary>
    public int SourcePlayerClientID;
    
    /// <summary>
    /// Le centre de l’explosion / l'endroit où l'ennemi a été touché
    /// </summary>
    public Vector3 Point;  
    
    //degats de zone
    
    /// <summary>
    /// Le rayon de l’explosion
    /// </summary>
    public float Radius;

    public DamageData(float amount, Vector3 point,int sourcePlayerClientID = -1, float radius = 0)
    {
        Amount = amount;
        SourcePlayerClientID = sourcePlayerClientID;
        Point = point;
        Radius = radius;
    }
}
