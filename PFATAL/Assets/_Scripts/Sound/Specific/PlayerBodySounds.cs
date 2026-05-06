using _scripts.PlayerCharacter;
using NetworkTime;
using UnityEngine;
using state = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class PlayerBodySounds : SoundComponent<PlayerAnimationEventsListener>
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
        if (!_playerCharacter.IsOwner)
        {
            main.OnFootstep += PlayFootstepSound;
        }

        _playerCharacter.replicatedStateMachineCallbacks.OnStateChanged += StateChanged_Callback;

        _playerCharacter.health.OnDamageTaken += (_) => PlayDamageSound();
    }


    void StateChanged_Callback(state previousState, state newState)
    {
        if ((previousState == state.Falling) && ((newState & state.Grounded) == state.Grounded))
            PlayLandSound();

        if (newState == state.Jumping)
            PlayJumpSound();

        if ((previousState == state.GroundSlam) && (newState == state.Idle))
            PlayGroundSlamSound();
    }

    void PlayGroundSlamSound() => AudioManager.Instance.PlayOnlineOneShots(Sounds.GroundSlamHit, Sounds.GroundSlamHit3D, transform.position);

    void PlayFootstepSound()
    {
        SwapGroundType();

        AudioManager.Instance.PlayOneShot(Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
    }

    void PlayLandSound()
    {
        SwapGroundType() ;

        AudioManager.Instance.PlayOnlineOneShots(Sounds.Footsteps, Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
        //AudioManager.Instance.PlayOneShot(Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
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
