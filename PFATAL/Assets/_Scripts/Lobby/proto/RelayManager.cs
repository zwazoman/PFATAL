using System;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }

    private const int MAX_CONNECTIONS = 7; // 8 joueurs max = 1 host + 7 clients

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<string> CreateRelayAllocation()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_CONNECTIONS);
            Debug.Log($"[Relay] Allocation créée. Region: {allocation.Region}");

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"[Relay] Code de jointure généré: {joinCode}");

            SetupHostTransport(allocation);

            return joinCode;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Relay] Erreur lors de la création de l'allocation: {e.Message}");
            return null;
        }
    }

    public async Task<bool> JoinRelayAllocation(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log($"[Relay] Allocation rejointe. Region: {allocation.Region}");

            SetupClientTransport(allocation);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Relay] Erreur lors de la jointure de l'allocation: {e.Message}");
            return false;
        }
    }

    private void SetupHostTransport(Allocation allocation)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetHostRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        Debug.Log("[Relay] Transport configuré pour l'host");
    }
    private void SetupClientTransport(JoinAllocation allocation)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetClientRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData
        );

        Debug.Log("[Relay] Transport configuré pour le client");
    }
}