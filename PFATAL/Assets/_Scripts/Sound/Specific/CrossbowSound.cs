using FMOD.Studio;
using UnityEngine;

[RequireComponent(typeof (Crossbow))]
public class CrossbowSound : SoundComponent<Crossbow>
{
    EventInstance _chargedCrossbowCharged;

    override protected void LinkEvents()
    {
        main.OnStartCharging += StartCharging_Callback;
        main.OnCharged += Charged_Callback;
        main.OnStopUsing += Charged_Callback;
        main.OnShoot += Shoot_Callback;
    }

    void StartCharging_Callback()
    {
        _chargedCrossbowCharged = AudioManager.Instance.CreateInstance(Sounds.CrossbowCharge);
        _chargedCrossbowCharged.start();
    }

    void Charged_Callback()
    {
        _chargedCrossbowCharged.stop(STOP_MODE.ALLOWFADEOUT);
        _chargedCrossbowCharged.release();
    }

    void Shoot_Callback()
    {
        AudioManager.Instance.PlayOnlineOneShots(Sounds.CrossbowShoot, Sounds.CrossbowShoot3D, transform.position/*, main.playerCharacter.OwnerClientId*/);
    }
}
