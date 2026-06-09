using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    private Unity.Services.Lobbies.Models.Lobby currentLobby;
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
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, "") }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS, options);
            return currentLobby.LobbyCode;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<bool> JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> JoinLobbyById(string lobbyId)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            return true;
        }
        catch (Exception e)
        {
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
            return true;
        }
        catch (Exception e)
        {
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
                return relayCode;
            }
            
            return null;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task SetLobbyLocked(bool locked)
    {
        if (currentLobby == null || !IsHost()) return;

        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
            {
                IsLocked = locked
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"[Lobby] Erreur lors du changement de verrou du lobby: {e.Message}");
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
            var lobbyId = currentLobby.Id;
            var playerId = UnityServicesManager.Instance.GetPlayerId();
            currentLobby = null;
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
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
            var lobbyId = currentLobby.Id;
            currentLobby = null;
            await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
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
                _ = DeleteLobby();
            else
                _ = LeaveLobby();
        }
    }
}