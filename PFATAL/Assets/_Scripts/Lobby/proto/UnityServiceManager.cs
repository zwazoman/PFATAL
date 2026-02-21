using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

public class UnityServicesManager : MonoBehaviour
{
    public static UnityServicesManager Instance { get; private set; }

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

    public async Task<bool> InitializeUnityServices()
    {
        try
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
                Debug.Log("[UnityServices] Initialisé avec succès");
            }

            // Authentification anonyme
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"[Authentication] Connecté avec ID: {AuthenticationService.Instance.PlayerId}");
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[UnityServices] Erreur d'initialisation: {e.Message}");
            return false;
        }
    }

    public string GetPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }

    public bool IsSignedIn()
    {
        return AuthenticationService.Instance.IsSignedIn;
    }
}