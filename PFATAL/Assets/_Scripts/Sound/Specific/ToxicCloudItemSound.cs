using UnityEngine;

public class ToxicCloudItemSound : SoundComponent<Cons_ToxicCloud>
{
    protected override void LinkEvents()
    {
        main.OnStopUsing += () => AudioManager.Instance.PlayOneShot(Sounds.SmokeLaunch);
    }
}
