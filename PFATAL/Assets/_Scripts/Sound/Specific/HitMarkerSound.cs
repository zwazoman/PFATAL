using UnityEngine;

[RequireComponent(typeof(HitMarkerUI))]
public class HitMarkerSound : SoundComponent<HitMarkerUI>
{
    private void Start()
    {
        main.OnShowHitMarker += ShowHitMarker_Callback;
    }

    void ShowHitMarker_Callback()
    {
        AudioManager.Instance.PlayOneShot(Sounds.HitMarker);
    }
}
