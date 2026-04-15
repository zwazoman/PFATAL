using _scripts.PlayerCharacter;
using FMODUnity;
using UnityEngine;

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

    }

    private void Update()
    {
        if (!_autoPlayTest)
            return;

        _timer += Time.deltaTime;
        if(_timer >= 2)
        {
            _timer = 0;
            PlayFootstepSound();
        }

    }


    void PlayFootstepSound()
    {
        _currentGroundType = CheckGroundType();

        print($"play {_currentGroundType.ToString()} footstep sound");

        AudioManager.Instance.PlayOneShot(Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
    }

    GroundType CheckGroundType()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, _groungCheckLength, _groundCheckLayerMask))
        {
            if(hit.collider.gameObject.TryGetComponent(out MeshRenderer mshRenderer))
            {
                print(mshRenderer.sharedMaterial.name);

                if (mshRenderer.sharedMaterial == _rockMaterial)
                    return GroundType.Rock;
                if(mshRenderer.sharedMaterial == _grassMaterial)
                    return GroundType.Grass;
                if(mshRenderer.sharedMaterial == _woodMaterial)
                    return GroundType.Wood;
            }
        }

        return GroundType.Rock;
    }

}

public enum GroundType
{
    Grass,
    Rock,
    Wood
}
