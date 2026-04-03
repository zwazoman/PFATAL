using UnityEngine;

[RequireComponent (typeof(Explosion))]
public class ExplosionSound : SoundComponent<Explosion>
{
    override protected void LinkEvents()
    {
        main.EventOnExplode += OnExplode_Callback;
    }

    void OnExplode_Callback()
    {
        AudioManager.Instance.PlayOneShot(Sounds.Explosion3D, transform.position);
    }
}
