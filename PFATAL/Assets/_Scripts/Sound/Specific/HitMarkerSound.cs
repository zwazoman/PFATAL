using UnityEngine;

[RequireComponent(typeof(HitMarkerUI))]
public class HitMarkerSound : SoundComponent<HitMarkerUI>
{
    protected override void LinkEvents()
    {
        main.OnShowHitMarker += ShowHitMarker_Callback;
    }

    void ShowHitMarker_Callback()
    {
        AudioManager.Instance.PlayOneShot(Sounds.HitMarker);
    }
}
