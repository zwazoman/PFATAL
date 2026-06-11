using System.Collections.Generic;
using _Scripts.Extensions;
using _scripts.PlayerCharacter;
using _Scripts.Pooling;
using DG.Tweening;
using SimpleVFXs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using State = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class PlayerCharacterVisuals : NetworkBehaviour
{
    private static readonly int StateAnimatorPropertyIndex = Animator.StringToHash("State");
    private static readonly int StrengthShaderPropertyIndex = Shader.PropertyToID("_strength");
    private static readonly int IsFallingAnimatorPropertyIndex = Animator.StringToHash("isFalling");
    private static readonly int IsRunningAnimatorPropertyIndex = Animator.StringToHash("IsRunning");
    
    public bool IsProxy => !IsOwner;
    public bool IsInFpsView => IsOwner;
    
    public static int fpsLayerMask;
    public static int defaultLayerMask;
    

    [Header("scene references")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] StylisedEffect _healVFX;
    [SerializeField] Transform _cameraRoot;
    [SerializeField] SkinHandler _skinHandler;
    [SerializeField] Animator _handAdditiveAnimator;
    
    [Header("FPS visuals")]
    [SerializeField] List<GameObject> _fpsVisuals;
    [SerializeField] MeshRenderer _swordSlash0;
    [SerializeField] MeshRenderer _swordSlash1;
    [SerializeField] MeshRenderer _swordDash;
    
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

    //VFXs
    public async void PlaySwordSlashAnimationVFX(float delay,bool firstSlash)
    {
        await Awaitable.WaitForSecondsAsync(delay);
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        float strength = .5f;
        Renderer renderer = firstSlash ? _swordSlash0 : _swordSlash1;
        renderer.enabled = true;
        DOTween.To(
            () => strength,
            (float x) =>
            {
                strength = x;
                materialPropertyBlock.SetFloat(StrengthShaderPropertyIndex, x);
                renderer.SetPropertyBlock(materialPropertyBlock);
            },
            0f, .25f).SetEase(Ease.OutQuad)
            .onComplete = () => renderer.enabled = false;
    }
    public async void PlaySwordDashAnimationVFX()
    {
        await Awaitable.WaitForSecondsAsync(.12f);
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        float strength = .5f;
        Renderer renderer = _swordDash;
        renderer.enabled = true;
        DOTween.To(
                () => strength,
                (float x) =>
                {
                    strength = x;
                    materialPropertyBlock.SetFloat(StrengthShaderPropertyIndex, x);
                    renderer.SetPropertyBlock(materialPropertyBlock);
                },
                0f, .55f).SetEase(Ease.OutCubic)
            .onComplete = () => renderer.enabled = false;
    }
    public void PlayHealingVFX()
    {
        _healVFX.vfx.SetFloat("Radius", IsInFpsView ? 1 : .5f);
        _healVFX.TriggerMainEvent();
    }
    
    //visibility
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

    //skins
    [Rpc(SendTo.Everyone)]
    public void SwapSkinRpc(int skinID)
    {
        _skinHandler.SwapSkin(skinID);
    }
    
    //feets & torso rotation
    async void Update()
    {
        Vector3 oldPosition = transform.position;
        
        if (IsProxy)
        {
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
        }
        
        //hands anim speed
        float walkSpeedMultiplier = _measuredVelocity.XZ().sqrMagnitude / (_playerCharacter.stateMachine.s_Walking._walkSpeed * _playerCharacter.stateMachine.s_Walking._walkSpeed);
        walkSpeedMultiplier = walkSpeedMultiplier * .5f + .5f;
        _handAdditiveAnimator.SetFloat("walkSpeed",walkSpeedMultiplier);

        //measure velocity
        await Awaitable.NextFrameAsync();
        _measuredVelocity = (transform.position - oldPosition)/Time.deltaTime;
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
        _playerCharacter.playerHands.leftHand.OnEquipItem += (_) => _handAdditiveAnimator.SetTrigger("pickupItem");
        _playerCharacter.playerHands.rightHand.OnEquipItem += (_) => _handAdditiveAnimator.SetTrigger("pickupWeapon");
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
        if(IsOwner)
            SwapSkinRpc(_skinHandler.GetCurrentSkinID());
    }
    
    

    //states
    private void OnStateChanged(State oldState, State newState)
    { 
        //tp proxy animation
        _proxyAnimator.SetInteger(StateAnimatorPropertyIndex,(int)newState);
        
        //walk particles
        if ((newState & State.Walking) == State.Walking)
        {
            _handAdditiveAnimator.SetBool(IsRunningAnimatorPropertyIndex,true);
            _walkVFX.Play();
        }
        
        if ((oldState & State.Walking) == State.Walking)
        {
            _handAdditiveAnimator.SetBool(IsRunningAnimatorPropertyIndex,false);
            _walkVFX.Stop();
        }
            
        
        //landing particles
        if ((oldState & State.Airborne) == State.Airborne
            && (newState & State.Grounded) == State.Grounded)
            _jumpVFX.Play();
        
        //jump particles
        if ((newState & State.Jumping) == State.Jumping)
        {
            _handAdditiveAnimator.SetBool(IsFallingAnimatorPropertyIndex,true);
            _jumpVFX.Play();
        }
        
        if ((newState & State.Falling) == State.Falling)
        {
            _handAdditiveAnimator.SetBool(IsFallingAnimatorPropertyIndex,true);
        }
        if ((oldState & State.Falling) == State.Falling)
        {
            _handAdditiveAnimator.SetBool(IsFallingAnimatorPropertyIndex,false);
        }
        
        //death particles
        if ((newState & State.Dead) == State.Dead)
            LocalPoolManager.Instance.Pool_VFX_RockBurst_Big.
                PullObjectFromPool(transform.position)
                .GoBackIntoPool_Delayed(2);

        //groundslam vfx
        if ((oldState & State.GroundSlam) == State.GroundSlam)
        {
            PooledObject vfx = LocalPoolManager.Instance.Pool_VFX_GroundSlam.PullObjectFromPool(transform.position+Vector3.down*.5f);
            vfx.GetComponent<StylisedEffect>().TriggerMainEvent();
            vfx.GoBackIntoPool_Delayed(3);
        }
    }

    private void OnDamageTaken(DamageData damageData)
    {
        //hit vfxs
        LocalPoolManager.Instance.Pool_VFX_Hit_crit.
            PullObjectFromPool(damageData.Point, Quaternion.LookRotation(-damageData.Direction))
            .GoBackIntoPool_Delayed(1.5f);
        LocalPoolManager.Instance.Pool_VFX_RockBurst_Small.
            PullObjectFromPool(damageData.Point, Quaternion.LookRotation(-damageData.Direction,Vector3.up))
            .GoBackIntoPool_Delayed(2f);
    }
}
