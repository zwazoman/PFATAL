using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyBrowser : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxResults = 10;
    [SerializeField] private bool filterFullLobbies = true;
    [SerializeField] private bool filterInGameLobbies = true;

    public event Action<List<Lobby>> OnLobbiesRefreshed;

    private List<Lobby> availableLobbies = new List<Lobby>();


    private void Start()
    {
        _ = RefreshLobbies();
    }

    public async Awaitable RefreshLobbies()
    {
        while (!UnityServicesManager.Instance.IsInitialized)
            await Awaitable.NextFrameAsync();

        try
        {
            QueryLobbiesOptions options = BuildQueryOptions();
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);

            availableLobbies = response.Results ?? new List<Lobby>();

            //Debug.Log($"[LobbyBrowser] {availableLobbies.Count} lobby(s) trouvé(s).");

            OnLobbiesRefreshed?.Invoke(availableLobbies);
        }
        catch (Exception e)
        {
            Debug.LogError($"[LobbyBrowser] Erreur lors de la recherche de lobbies: {e.Message}");
        }
    }

    public async Task<bool> JoinLobby(Lobby lobby)
    {
        QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

        foreach (Lobby lobbys in queryResponse.Results)
        {
            Debug.Log("lobby code: " + lobbys.LobbyCode);
        }
        await Task.Delay(100); 
        
        if (lobby == null)
        {
            return false;
        }
        else if (lobby.Players.Count >= lobby.MaxPlayers)
        {
            return false;
        }
        else if (lobby.LobbyCode == null)
        {
            return false;
        }
        await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
        return await NetworkConnectionManager.Instance.StartClient(lobby.LobbyCode);
        
    }

    private QueryLobbiesOptions BuildQueryOptions()
    {
        var filters = new List<QueryFilter>();

        // Filtre les lobbies pleins si activé
        if (filterFullLobbies)
        {
            filters.Add(new QueryFilter(
                field: QueryFilter.FieldOptions.AvailableSlots,
                op: QueryFilter.OpOptions.GT,
                value: "0"
            ));
        }
        
        //Filtre les lobbies bloquér ou non 
        if (filterInGameLobbies)
        {
            filters.Add(new QueryFilter(
                field: QueryFilter.FieldOptions.IsLocked,
                op: QueryFilter.OpOptions.EQ,
                value: "0"
            ));
        }

        return new QueryLobbiesOptions
        {
            Count = maxResults,
            Filters = filters,
            Order = new List<QueryOrder>
            {
                // Lobbies les plus récents en premier
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created
                )
            }
        };
    }
}