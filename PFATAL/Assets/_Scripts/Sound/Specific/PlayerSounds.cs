using _scripts.PlayerCharacter;
using FMODUnity;
using UnityEngine;
using state = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class PlayerSounds : SoundComponent<PlayerAnimationEventsListener>
{
    [SerializeField] PlayerCharacter _playerCharacter;

    [Header("Check Settings")]
    [SerializeField] float _groungCheckLength;
    [SerializeField] LayerMask _groundCheckLayerMask;

    [Header("Ground Materials")]
    [SerializeField] Material _rockMaterial;
    [SerializeField] Material _grassMaterial;
    [SerializeField] Material _woodMaterial;

    GroundType _currentGroundType;

    bool _autoPlayTest = false;
    float _timer;

    protected override void LinkEvents()
    {
        base.LinkEvents();

        if (!_playerCharacter.IsOwner)
            main.OnFootstep += PlayFootstepSound;
        else
            _autoPlayTest = true;

        _playerCharacter.replicatedStateMachineCallbacks.OnStateChanged += StateChanged_Callback;

        _playerCharacter.health.OnDamageTaken += (_) => PlayDamageSound();
    }

    private void Update()
    {
        if (!_autoPlayTest)
            return;

        _timer += Time.deltaTime;
        if (_timer >= 2)
        {
            _timer = 0;
            //PlayFootstepSound();
        }
    }


    void StateChanged_Callback(state previousState, state newState)
    {
        if ((previousState == state.Falling) && ((newState & state.Grounded) == state.Grounded))
            PlayLandSound();

        if (((previousState & state.Grounded) == state.Grounded) && (newState == state.Jumping))
            PlayJumpSound();
    }

    void PlayFootstepSound()
    {
        SwapGroundType();

        AudioManager.Instance.PlayOneShot(Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
    }

    void PlayLandSound()
    {
        SwapGroundType() ;

        AudioManager.Instance.PlayOneShot(Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
    }

    void PlayJumpSound() => AudioManager.Instance.PlayOneShot(Sounds.Jump);

    void PlayDamageSound() => AudioManager.Instance.PlayOneShot(Sounds.Hurt);

    void SwapGroundType()
    {
        _currentGroundType = CheckGroundType();
    }

    GroundType CheckGroundType()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, _groungCheckLength, _groundCheckLayerMask))
        {
            if (hit.collider.gameObject.TryGetComponent(out MeshRenderer mshRenderer))
            {
                //print(mshRenderer.sharedMaterial.name);

                if (mshRenderer.sharedMaterial == _rockMaterial)
                    return GroundType.Rock;
                if (mshRenderer.sharedMaterial == _grassMaterial)
                    return GroundType.Grass;
                if (mshRenderer.sharedMaterial == _woodMaterial)
                    return GroundType.Wood;
            }
        }

        return GroundType.Rock;
    }

    public enum GroundType
    {
        Grass,
        Rock,
        Wood
    }

}
