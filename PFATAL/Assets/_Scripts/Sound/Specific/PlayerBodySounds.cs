using _scripts.PlayerCharacter;
using _scripts.PlayerCharacter.StateMachine.States;
using _Scripts.StateMachine;
using NetworkTime;
using Unity.VisualScripting;
using UnityEngine;
using state = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class PlayerBodySounds : SoundComponent<PlayerAnimationEventsListener>
{
    [SerializeField] PlayerCharacter _playerCharacter;

    [Header("Check Settings")]
    [SerializeField] float _groungCheckLength;
    [SerializeField] LayerMask _groundCheckLayerMask;

    [Header("Ground Materials")]
    [SerializeField] Material[] _rockMaterials;
    [SerializeField] Material[] _grassMaterials;
    [SerializeField] Material[] _woodMaterials;

    [Header("FootSteps Settings")]
    float _footstepsDelay = .4f;

    GroundType _currentGroundType;

    bool _isWalking = false;
    float _timer;
    float _footstepsTimer;

    protected override void LinkEvents()
    {
        if (!_playerCharacter.IsOwner)
        {
            main.OnFootstep += Play3DFootstepSound;
        }

        _playerCharacter.replicatedStateMachineCallbacks.OnStateChanged += ReplicatedStateChanged_Callback;
        _playerCharacter.stateMachine.OnStateChanged += LocalStateCHanged_Callback;

        _playerCharacter.health.OnLocalDamageTaken += PlayDamageSound;
    }

    private void Update()
    {
        if (!_isWalking || !AudioManager.Instance.playSounds)
            return;
            
        _footstepsTimer += Time.deltaTime;

        if (_footstepsTimer >= _footstepsDelay)
        {
            PlayFootstepSound();
            _footstepsTimer = Random.Range(-0.1f, 0.1f);
        }
    }

    void LocalStateCHanged_Callback(StateBase<PlayerCharacter> previousState, StateBase<PlayerCharacter> newState)
    {
        if (newState is Pst_Walking)
        {
            _isWalking = true;
            PlayFootstepSound();
        }
        else
        {
            _isWalking = false;
            _footstepsTimer = 0;
        }

        if ((previousState is Pst_Falling) && newState is Pst_Grounded)
            PlayLandSound();

        if (newState is Pst_Jumping)
            PlayJumpSound();

        if (previousState is Pst_GroundSlam)
            PlayGroundSlamSound();
    }


    void ReplicatedStateChanged_Callback(state previousState, state newState)
    {

    }

    void PlayFootstepSound()
    {
        SwapGroundType();
        AudioManager.Instance.PlayOnlineOneShots(Sounds.Footsteps, Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
    }

    void PlayGroundSlamSound() => AudioManager.Instance.PlayOnlineOneShots(Sounds.GroundSlamHit, Sounds.GroundSlamHit3D, transform.position);

    void Play3DFootstepSound()
    {
        SwapGroundType();
        AudioManager.Instance.PlayOneShot(Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
    }

    void PlayLandSound()
    {
        SwapGroundType() ;
        AudioManager.Instance.PlayOnlineOneShots(Sounds.Footsteps, Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
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

                foreach(Material mat in _rockMaterials)
                {
                    if (mshRenderer.sharedMaterial == mat)
                        return GroundType.Rock;
                }

                foreach (Material mat in _grassMaterials)
                {
                    if (mshRenderer.sharedMaterial == mat)
                        return GroundType.Grass;
                }
                foreach (Material mat in _woodMaterials)
                {
                    if (mshRenderer.sharedMaterial == mat)
                        return GroundType.Wood;
                }

                //todo pas dans l'eau
            }
            

            if (hit.collider.gameObject.transform.GetChild(0))
            {
                if(hit.collider.gameObject.transform.GetChild(0).TryGetComponent(out MeshRenderer renderer))
                {
                    foreach (Material mat in _woodMaterials)
                    {
                        if (renderer.sharedMaterial == mat)
                            return GroundType.Wood;
                    }
                }
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
