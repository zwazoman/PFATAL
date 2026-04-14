using _scripts.PlayerCharacter;
using UnityEngine;


public class HeatMapAnalitycs : MonoBehaviour
{
    [SerializeField] private PlayerCharacter character;

    public GameObject playerWeaponSocket;

    public bool isOnCapsule = false;

    private void Start()
    {
        if (isOnCapsule)
        {
            if (!character.IsServer) return;
        }

        HeatMapServerAnalitics.instance.Players.Add(gameObject);
    }
}