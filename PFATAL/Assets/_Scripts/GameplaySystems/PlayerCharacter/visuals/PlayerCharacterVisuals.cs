using System.Collections.Generic;
using _scripts.PlayerCharacter;
using _Scripts.Pooling;
using SimpleVFXs;
using Unity.Netcode;
using UnityEngine;

using State = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class PlayerCharacterVisuals : NetworkBehaviour
{
    private static readonly int StateAnimatorPropertyIndex = Animator.StringToHash("State");
    public bool IsProxy => !IsOwner;
    public bool IsInFpsView => IsOwner;
    
    public static int fpsLayerMask;
    public static int defaultLayerMask;
    
    [Header("scene references")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] StylisedEffect _healVFX;
    [SerializeField] Transform _cameraRoot;
    
    [Header("FPS visuals")]
    [SerializeField] List<GameObject> _fpsVisuals;
    
    [Header("Third person Proxy visuals")]
    [SerializeField] List<GameObject> _proxyVisuals;
    [SerializeField] Animator _proxyAnimator;
    [SerializeField] Transform _proxyTorsoSocket;
    [SerializeField] Transform _proxyFeetSocket;
    [SerializeField] ParticleSystem _walkVFX;
    [SerializeField] ParticleSystem _jumpVFX;

    [Header("Settings")]
    [SerializeField] float TorsoPitchAmplitude = 75;

    private Vector3 _measuredVelocity;
    
    //public methods
    
    [SerializeField] List<GameObject> _visualObjects;

    public void PlayHealingVFX()
    {
        _healVFX.vfx.SetFloat("Radius", IsInFpsView ? 1 : .5f);
        _healVFX.TriggerMainEvent();
    }
    
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
        fpsLayerMask = LayerMask.NameToLayer("FpsViewOnly");
        defaultLayerMask = LayerMask.NameToLayer("Default");
        
        //enable fps view by default
        SetFPSViewEnabled(true);
        
        _playerCharacter.replicatedStateMachineCallbacks.OnStateChanged += OnStateChanged;
        
        //setup events
        _playerCharacter.health.OnDamageTaken += OnDamageTaken;
    }

    void SetFPSViewEnabled(bool enabled)
    {
        //update objects visibility
        foreach(GameObject obj in _proxyVisuals)
            obj.SetActive(!enabled);
        foreach(GameObject obj in _fpsVisuals)
            obj.SetActive(enabled);
        
        //update hands layer
        void SetLayerRecursive(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
            {
                SetLayerRecursive(child, layer);
            }
        }
        SetLayerRecursive(_playerCharacter.playerHands.leftHand.transform,IsInFpsView ? fpsLayerMask : defaultLayerMask);
        SetLayerRecursive(_playerCharacter.playerHands.rightHand.transform,IsInFpsView ? fpsLayerMask :defaultLayerMask);
        
        //update hands position
        _playerCharacter.playerHands.leftHand.transform.localPosition = enabled ?
            _playerCharacter.playerHands.leftHand.fpsPosition : 
            _playerCharacter.playerHands.leftHand.tpsPosition ;
        
        _playerCharacter.playerHands.rightHand.transform.localPosition = enabled ?
            _playerCharacter.playerHands.rightHand.fpsPosition : 
            _playerCharacter.playerHands.rightHand.tpsPosition ;
    }
    
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        SetFPSViewEnabled(IsInFpsView);
    }

    private void OnStateChanged(State oldState, State newState)
    { 
        //tp proxy animation
        _proxyAnimator.SetInteger(StateAnimatorPropertyIndex,(int)newState);
        
        //walk particles
        if ((newState & State.Walking) == State.Walking)
            _walkVFX.Play();
        else 
            _walkVFX.Stop();
        
        //landing particles
        if ((oldState & State.Airborne) == State.Airborne
            && (newState & State.Grounded) == State.Grounded)
            _jumpVFX.Play();
        
        //jump particles
        if ((newState & State.Jumping) == State.Jumping)
            _jumpVFX.Play();
    }

    private void OnDamageTaken(DamageData damageData)
    {
        //hit vfx
        LocalPoolManager.Instance.Pool_VFX_Hit_crit.
            PullObjectFromPool(damageData.Point, Quaternion.LookRotation(-damageData.Direction))
            .GoBackIntoPool_Delayed(1.5f);
    }
}
