using UnityEngine;

public class TPSound : SoundComponent<Cons_TP>
{
    protected override void LinkEvents()
    {
        main.OnStopUsing += PlaySound;
    }

    void PlaySound()
    {
        AudioManager.Instance.PlayOnlineOneShots(Sounds.TP, Sounds.TP3D, transform.position);
    }
}
