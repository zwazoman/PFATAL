using System;
using System.Threading.Tasks;
using _scripts.PlayerCharacter;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerSpawner : MonoBehaviour
{
//singleton
    public static PlayerSpawner Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }


    /// <summary>
    /// Doit être appelé par le server. le host récupère les personnages spawnés de son coté
    /// pour faire fonctionner les rêgles du jeu en les observant.
    /// </summary>
    /// <param name="ownerClientID"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<PlayerCharacter> SpawnPlayerCharacter(ulong ownerClientID)
    {
        //todo : nestor
        throw new NotImplementedException("demander à Nestor");
    }
}
