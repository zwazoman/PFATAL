using Unity.Netcode;
using UnityEngine;

public class NetworkStartUI : MonoBehaviour
{
    [SerializeField] PlayerCharacterSpawner _playerSpawner ;

    private async void OnGUI()
    {
        float w = 200f, h = 40f;
        float x = 10f, y = 10f;

        if (GUI.Button(new Rect(x, y, w, h), "Host")) NetworkManager.Singleton.StartHost();
        if (GUI.Button(new Rect(x, y + h + 10, w, h), "Client")) NetworkManager.Singleton.StartClient();
        if(GUI.Button(new Rect(x, y+2*h+10,w,h), "Spawn Players") && NetworkManager.Singleton.IsServer)
        {
            foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                await _playerSpawner.SpawnInitialPlayerCharacter(clientId);
            }
        }
    }
}
