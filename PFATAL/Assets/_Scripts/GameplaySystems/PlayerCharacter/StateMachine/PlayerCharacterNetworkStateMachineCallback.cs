using System;
using System.Collections.Generic;
using _scripts.PlayerCharacter;
using _scripts.PlayerCharacter.StateMachine.States;
using _Scripts.StateMachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// cette classe permet de recevoir des events liés à la state machine sur tous les clients,
/// et pas juste le owner de la state machine.
/// </summary>
public class PlayerCharacterNetworkStateMachineCallback : NetworkBehaviour
{
    public enum PlayerStateEnum
    {
        Unknown =  0,
        
        Alive = 1,
        Dead = 2,
        GameOver = 4,
        
        Grounded = 8 | Alive,
        Airborne = 16 | Alive,
        Frozen = 32 | Alive,
        
        Idle = 64 | Grounded,
        Walking = 128| Grounded,
        Falling = 256 | Airborne,
        Jumping = 512 | Airborne,
    }
    
    [Header("scene references")]
    [SerializeField] PlayerStateMachine _stateMachine;
    
    //synced variables and events
    public event Action<PlayerStateEnum,PlayerStateEnum> OnStateChanged;
    public PlayerStateEnum CurrentState { get;private set; }
    public PlayerStateEnum OldState{ get;private set; }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            _stateMachine.OnStateChanged += SerializeAndReplicateStateChangedEventRPC;
        }
    }

    void SerializeAndReplicateStateChangedEventRPC(StateBase<PlayerCharacter> from, StateBase<PlayerCharacter> to)
    {
        ReplicateStateChangedEventRPC(
            GetTypeEnumForStateObject(from),
            GetTypeEnumForStateObject(to));
    }
        
    PlayerStateEnum GetTypeEnumForStateObject(StateBase<PlayerCharacter> stateObject)
    {
        return stateObject switch
        {
            //concrets avec heritage
            Pst_Falling => PlayerStateEnum.Falling,
            Pst_Idle => PlayerStateEnum.Idle,
            Pst_Walking => PlayerStateEnum.Walking,
            Pst_Jumping => PlayerStateEnum.Jumping,
            Pst_Frozen => PlayerStateEnum.Frozen,
            
            //abstraits ( dans l'ordre )
            Pst_Grounded => PlayerStateEnum.Grounded,
            Pst_Airborne => PlayerStateEnum.Airborne,
            Pst_Alive => PlayerStateEnum.Alive,
            
            //concrets sans heritage
            Pst_Dead => PlayerStateEnum.Dead,
            Pst_GameOver => PlayerStateEnum.GameOver,
            
            _ => PlayerStateEnum.Unknown
        };
    }

    [Rpc(SendTo.Everyone)]
    void ReplicateStateChangedEventRPC(PlayerStateEnum from, PlayerStateEnum to)
    {
        OldState = from;
        CurrentState = to;
        OnStateChanged?.Invoke(from,to);
    }
}
