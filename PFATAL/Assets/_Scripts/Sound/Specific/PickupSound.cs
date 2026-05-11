using UnityEngine;

public class PickupSound : SoundComponent<Pickup>
{
    //protected override async void Awake()
    //{
    //    await Awaitable.NextFrameAsync();
    //    base.Awake();
    //}

    protected override void LinkEvents()
    {
        main.OnPickup += Pickup_Callback;
    }

    void Pickup_Callback() => AudioManager.Instance.PlayOneShot(Sounds.Pickup);
}
