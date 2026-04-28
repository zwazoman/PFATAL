using UnityEngine;

public class BearTrapSound : SoundComponent<Proj_WolfTrap>
{
    protected override void LinkEvents()
    {
        main.OnTrapPlayer += TrapPlayer_Callback;
    }

    void TrapPlayer_Callback()
    {
        AudioManager.Instance.PlayOneShotForEveryoneRPC(Sounds.BearTrapSetup3D, transform.position);
    }
}
