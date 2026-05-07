using UnityEngine;

[RequireComponent(typeof(HitMarkerUI))]
public class HitMarkerSound : SoundComponent<HitMarkerUI>
{
    protected override void LinkEvents()
    {
        main.OnShowHitMarker += ShowHitMarker_Callback;
        main.OnShowKillMarker += ShowKillMarker_Callback;
    }

    void ShowKillMarker_Callback()
    {
        AudioManager.Instance.PlayOneShot(Sounds.Kill);
    }

    void ShowHitMarker_Callback()
    {
        AudioManager.Instance.PlayOneShot(Sounds.HitMarker);
    }
}
