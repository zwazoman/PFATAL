using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Envoie un ping à tous les clients régulièrement.
/// </summary>
public class HostHeartbeat : NetworkBehaviour
{
    [SerializeField] private float heartbeatInterval = 2f; // Envoie un signal toutes les 2s
    private float _timer;

    private void Update()
    {
        if (!IsHost) return;

        _timer += Time.deltaTime;
        if (_timer >= heartbeatInterval)
        {
            _timer = 0f;
            SendHeartbeatClientRpc();
        }
    }

    [ClientRpc]
    private void SendHeartbeatClientRpc()
    {
        if (IsHost) return;
        
        ClientConnectionWatcher.Instance?.OnHeartbeatReceived();
    }
}