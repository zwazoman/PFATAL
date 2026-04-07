
using System.Collections.Generic;
using _scripts.PlayerCharacter;
using _Scripts.Pooling;
using Unity.Netcode;
using UnityEngine;

using State = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class PlayerCharacterVisuals : NetworkBehaviour
{
    private static readonly int StateAnimatorPropertyIndex = Animator.StringToHash("State");
    public bool IsProxy => !IsOwner;
    public bool IsInFpsView => IsOwner;
    
    [Header("scene references")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] Transform _cameraRoot;
    
    [Header("FPS visuals")]
    [SerializeField] List<GameObject> _fpsVisuals;
    
    [Header("Third person Proxy visuals")]
    [SerializeField] List<GameObject> _proxyVisuals;
    [SerializeField] Animator _proxyAnimator;
    [SerializeField] Transform _proxyTorsoSocket;
    [SerializeField] Transform _proxyFeetSocket;

    [Header("Settings")]
    [SerializeField] float TorsoPitchAmplitude = 75;

    private Vector3 _measuredVelocity;
    
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

    async void Update()
    {
        if (IsProxy)
        {
            Vector3 oldPosition = transform.position;
            
            //feet rotation
            if (_measuredVelocity.magnitude > .1f)
            {
                //Vector3 localSpaceVelocity = transform.TransformVector(_measuredVelocity);
                _proxyFeetSocket.transform.rotation = 
                    Quaternion.Euler(
                        -90, Mathf.Atan2(_measuredVelocity.z,-_measuredVelocity.x)*Mathf.Rad2Deg-90,0);
            }
            
            //torso rotation
            _proxyTorsoSocket.transform.localRotation = Quaternion.Euler(90+_cameraRoot.transform.localRotation.x*TorsoPitchAmplitude,0,0);
            
            //measure velocity
            await Awaitable.NextFrameAsync();
            _measuredVelocity = (transform.position - oldPosition)/Time.deltaTime;
        }
    }
    
    void Awake()
    {
        //enable fps view by default
        SetFPSViewEnabled(true);
        
        //setup events
        _playerCharacter.health.OnDamageTaken += OnDamageTaken;
        _playerCharacter.replicatedStateMachineCallbacks.OnStateChanged += OnStateChanged;
    }

    void SetFPSViewEnabled(bool enabled)
    {
        foreach(GameObject obj in _proxyVisuals)
            obj.SetActive(!enabled);
        foreach(GameObject obj in _fpsVisuals)
            obj.SetActive(enabled);
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        SetFPSViewEnabled(IsInFpsView);
    }

    private void OnStateChanged(State oldState, State newState)
    { 
        _proxyAnimator.SetInteger(StateAnimatorPropertyIndex,(int)newState);
    }

    private void OnDamageTaken(DamageData damageData)
    {
        //vfx
        LocalPoolManager.Instance.Pool_VFX_Hit_crit.
            PullObjectFromPool(damageData.Point, Quaternion.LookRotation(-damageData.Direction))
            .GoBackIntoPool_Delayed(1.5f);
    }
}
