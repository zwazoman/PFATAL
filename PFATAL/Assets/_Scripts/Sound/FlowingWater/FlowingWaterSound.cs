using UnityEngine;

public class FlowingWaterSound : MovingSoundComponent<FlowingRiver>
{
    protected override void LinkEvents()
    {
        StartSound(Sounds.SplashAmbience);
    }
}
