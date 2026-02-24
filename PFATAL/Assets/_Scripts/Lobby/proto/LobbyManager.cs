using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    private Lobby currentLobby;
    private const int MAX_PLAYERS = 8;
    private const string RELAY_JOIN_CODE_KEY = "RelayJoinCode";

    private float heartbeatTimer = 0f;
    private const float HEARTBEAT_INTERVAL = 15f;

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

    private void Update()
    {
        HandleLobbyHeartbeat();
    }


    public async Task<string> CreateLobby(string lobbyName = "MyGame")
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Data = new Dictionary<string, DataObject>
                {
                    { RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, "") }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS, options);
            Debug.Log($"[Lobby] Lobby créé avec le code: {currentLobby.LobbyCode}");

            return currentLobby.LobbyCode;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Lobby] Erreur lors de la création du lobby: {e.Message}");
            return null;
        }
    }

    public async Task<bool> JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            Debug.Log($"[Lobby] Lobby rejoint: {currentLobby.Name} ({currentLobby.Players.Count}/{currentLobby.MaxPlayers} joueurs)");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Lobby] Erreur lors de la jointure du lobby: {e.Message}");
            return false;
        }
    }
    public async Task<bool> UpdateLobbyRelayCode(string relayJoinCode)
    {
        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            };

            currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, options);
            Debug.Log($"[Lobby] Code Relay mis à jour dans le lobby");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Lobby] Erreur lors de la mise à jour du code Relay: {e.Message}");
            return false;
        }
    }

    public async Task<string> GetRelayJoinCode()
    {
        try
        {
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

            if (currentLobby.Data.ContainsKey(RELAY_JOIN_CODE_KEY))
            {
                string relayCode = currentLobby.Data[RELAY_JOIN_CODE_KEY].Value;
                Debug.Log($"[Lobby] Code Relay récupéré: {relayCode}");
                return relayCode;
            }

            Debug.LogWarning("[Lobby] Aucun code Relay trouvé dans le lobby");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Lobby] Erreur lors de la récupération du code Relay: {e.Message}");
            return null;
        }
    }
    private async void HandleLobbyHeartbeat()
    {
        if (currentLobby != null && IsHost())
        {
            heartbeatTimer += Time.deltaTime;

            if (heartbeatTimer >= HEARTBEAT_INTERVAL)
            {
                heartbeatTimer = 0f;

                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
                    Debug.Log("[Lobby] Heartbeat envoyé");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Lobby] Erreur heartbeat: {e.Message}");
                }
            }
        }
    }
    public async Task LeaveLobby()
    {
        if (currentLobby == null) return;

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, UnityServicesManager.Instance.GetPlayerId());
            Debug.Log("[Lobby] Lobby quitté");
            currentLobby = null;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Lobby] Erreur lors de la sortie du lobby: {e.Message}");
        }
    }

    public async Task DeleteLobby()
    {
        if (currentLobby == null || !IsHost()) return;

        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
            Debug.Log("[Lobby] Lobby supprimé");
            currentLobby = null;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Lobby] Erreur lors de la suppression du lobby: {e.Message}");
        }
    }
    public bool IsHost()
    {
        return currentLobby != null && currentLobby.HostId == UnityServicesManager.Instance.GetPlayerId();
    }

    public Lobby GetCurrentLobby()
    {
        return currentLobby;
    }

    private void OnDestroy()
    {
        if (currentLobby != null)
        {
            if (IsHost())
            {
                _ = DeleteLobby();
            }
            else
            {
                _ = LeaveLobby();
            }
        }
    }
}