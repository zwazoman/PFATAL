
using System.Collections.Generic;
using _scripts.PlayerCharacter;
using _Scripts.Pooling;
using Unity.Netcode;
using UnityEngine;

using State = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class PlayerCharacterVisuals : NetworkBehaviour
{
    private static readonly int StateAnimatorPropertyIndex = Animator.StringToHash("State");

    [Header("scene references")]
    [SerializeField] PlayerCharacter _playerCharacter;
    
    [Header("FPS visuals")]
    [SerializeField] List<GameObject> _fpsVisuals;
    
    [Header("Third person Proxy visuals")]
    [SerializeField] List<GameObject> _proxyVisuals;
    [SerializeField] Animator _proxyAnimator;
    [SerializeField] Transform _proxyTorsoSocket;
    [SerializeField] Transform _proxyFeetSocket;

    
    //public methods

    [SerializeField] List<GameObject> _visualObjects;
        
    [Rpc(SendTo.Everyone)]
    public void HideRpc()
    {
        foreach (var obj in _visualObjects)
            obj.SetActive(false);
    }

    [Rpc(SendTo.Everyone)]
    public void ShowRpc()
    {
        foreach (var obj in _visualObjects)
            obj.SetActive(true);
    }
    
    //events

    void Update()
    {
        if(!IsOwner)
            _proxyFeetSocket.transform.rotation = Quaternion.Euler(90,0,Mathf.Atan2(_playerCharacter.physics.Velocity.z,-_playerCharacter.physics.Velocity.x)*Mathf.Rad2Deg);
    }
    
    void Awake()
    {
        _playerCharacter.health.OnDamageTaken += OnDamageTaken;
        _playerCharacter.replicatedStateMachineCallbacks.OnStateChanged += OnStateChanged;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        foreach(GameObject obj in _proxyVisuals)
            obj.SetActive(!IsOwner);
        
        foreach(GameObject obj in _fpsVisuals)
            obj.SetActive(IsOwner);
    }

    private void OnStateChanged(State oldState, State newState)
    { 
        _proxyAnimator.SetInteger(StateAnimatorPropertyIndex,(int)newState) ;
        
        //if((newState & State.Idle) == State.Idle)
    }

    private void OnDamageTaken(DamageData damageData)
    {
        //vfx
        LocalPoolManager.Instance.Pool_VFX_Hit_crit.
            PullObjectFromPool(damageData.Point, Quaternion.LookRotation(-damageData.Direction))
            .GoBackIntoPool_Delayed(1.5f);
    }
}
