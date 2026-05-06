using UnityEngine;

public class BearTrapSound : SoundComponent<Proj_WolfTrap>
{
    protected override void LinkEvents()
    {
        main.OnDeploy += TrapSetup_Callback;
        main.OnTrapPlayer += TrapTrigger_Callback;
    }

    void TrapSetup_Callback() => AudioManager.Instance.PlayOneShotForEveryoneRPC(Sounds.BearTrapSetup3D, transform.position);

    void TrapTrigger_Callback() => AudioManager.Instance.PlayOneShotForEveryoneRPC(Sounds.BearTrapTrigger3D, transform.position);

}
