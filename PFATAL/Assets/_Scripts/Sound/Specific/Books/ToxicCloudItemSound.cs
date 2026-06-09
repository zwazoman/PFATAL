using UnityEngine;

public class ToxicCloudItemSound : BookSound<Cons_ToxicCloud>
{
    protected override void LinkEvents()
    {
        base.LinkEvents();
        main.OnStopUsing += () => AudioManager.Instance.PlayOneShot(Sounds.SmokeLaunch);
    }

}
