using _scripts.PlayerCharacter;
using FMODUnity;
using UnityEngine;

public class PlayerSounds : SoundComponent<PlayerAnimationEventsListener>
{
    [SerializeField] PlayerCharacter _playerCharacter;

    GroundType _currentGroundType;

    protected override void LinkEvents()
    {
        base.LinkEvents();

        if (!_playerCharacter.IsOwner)
            main.OnFootstep += FootstepAnimation_Callback;
    }

    void FootstepAnimation_Callback()
    {
        _currentGroundType = CheckGroundType();

        AudioManager.Instance.PlayOneShot(Sounds.Footsteps3D, transform.position, "GroundType", (int)_currentGroundType);
    }

    GroundType CheckGroundType()
    {
        return GroundType.Grass;
    }

}

public enum GroundType
{
    Grass,
    Rock,
    Wood
}
