using UnityEngine;
using Steamworks;
using System;
using System.Text;

public class SteamAuthenticator : MonoBehaviour
{
    public static SteamAuthenticator Instance { get; private set; }

    public static event Action<CSteamID, string> OnAuthSuccess;
    public static event Action OnAuthFailed;

    private byte[] m_AuthTicket;
    private HAuthTicket m_HAuthTicket;
    private uint m_TicketSize;
    private bool m_IsAuthenticated = false;

    public static bool IsAuthenticated => Instance?.m_IsAuthenticated ?? false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[SteamAuth] Singleton initialisé");
        }
        else
        {
            Debug.LogWarning("[SteamAuth] Une instance existe déjà, destruction de l'objet dupliqué");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("[SteamAuth] SteamManager non initialisé !");
            OnAuthFailed?.Invoke();
            return;
        }

        Authenticate();
    }

    public void Authenticate()
    {
        CSteamID steamID = SteamUser.GetSteamID();
        if (steamID == CSteamID.Nil)
        {
            Debug.LogError("[SteamAuth] SteamID invalide !");
            OnAuthFailed?.Invoke();
            return;
        }

        string playerName = SteamFriends.GetPersonaName();

        m_AuthTicket = new byte[1024];
        SteamNetworkingIdentity identity = new SteamNetworkingIdentity();
        identity.SetSteamID(SteamUser.GetSteamID());
        m_HAuthTicket = SteamUser.GetAuthSessionTicket(m_AuthTicket, 1024, out m_TicketSize, ref identity);

        if (m_HAuthTicket == HAuthTicket.Invalid)
        {
            Debug.LogError("[SteamAuth] Impossible de générer l'Auth Ticket !");
            OnAuthFailed?.Invoke();
            return;
        }

        m_IsAuthenticated = true;

        Debug.Log($"[SteamAuth] Connecté ! SteamID : {steamID} | Pseudo : {playerName}");
        Debug.Log($"[SteamAuth] Auth Ticket généré ({m_TicketSize} bytes)");

        OnAuthSuccess?.Invoke(steamID, playerName);
    }

    void OnDestroy()
    {
        if (Instance == this && m_HAuthTicket != HAuthTicket.Invalid)
        {
            SteamUser.CancelAuthTicket(m_HAuthTicket);
            Debug.Log("[SteamAuth] Auth Ticket annulé");
        }
    }
}