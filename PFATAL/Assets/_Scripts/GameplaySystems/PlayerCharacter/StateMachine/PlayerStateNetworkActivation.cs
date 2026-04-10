using Unity.Netcode;

public class PlayerStateNetworkActivation : NetworkBehaviour
{
    private PlayerStateMachine _sm;

    private void Awake()
    {
        _sm = GetComponent<PlayerStateMachine>();
    }

    public void ApplyState(PlayerStateID state, float value = 0f, ulong ownerId = 0)
    {
        switch (state)
        {
            case PlayerStateID.Frozen:
                _sm.s_Frozen.Freeze(value);
                break;

            case PlayerStateID.Propulsed:
                _sm.s_PropulseInAir.ActivateState(ownerId);
                break;
        }
    }

    // appelé par le serveur
    public void ApplyStateServer(PlayerStateID state, float value = 0f, ulong ownerId = 0)
    {
        if (!IsServer) return;

        // appliquer sur le serveur
        ApplyState(state, value, ownerId);

        // applique sur les clients
        ApplyStateClientRpc(state, value, ownerId);
    }

    [ClientRpc]
    private void ApplyStateClientRpc(PlayerStateID state, float value, ulong ownerId)
    {
        if (IsServer) return;

        ApplyState(state, value, ownerId);
    }
}

public enum PlayerStateID
{
    Frozen,
    Propulsed
}
