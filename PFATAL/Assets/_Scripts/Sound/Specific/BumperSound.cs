using UnityEngine;

public class BumperSound : SoundComponent<Bumper>
{
    protected override void LinkEvents()
    {
        main.OnPlayerBounce += PlaySound;
    }

    void PlaySound()
    {
        AudioManager.Instance.PlayOneShotForEveryoneRPC(Sounds.MushroomBump3D, transform.position);
    }

}
