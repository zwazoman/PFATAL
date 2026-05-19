using UnityEngine;

public class TPSound : RuneSound<Cons_TP>
{
    protected override void UseSound()
    {
        base.UseSound();
        AudioManager.Instance.PlayOnlineOneShots(Sounds.TP, Sounds.TP3D, transform.position);
    }
}
