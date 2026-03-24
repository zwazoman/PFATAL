using FMOD.Studio;
using Unity.Netcode;
using UnityEngine;

public class CrossbowSound : NetworkBehaviour
{
    [SerializeField] Crossbow _crossbow;

    EventInstance _chargedCrossbowCharged;

    private void Awake()
    {
        TryGetComponent(out _crossbow);
    }

    private void Start()
    {
        _crossbow.OnStartCharging += StartCharging_Callback;
        _crossbow.OnCharged += Charged_Callback;
        _crossbow.OnStopUsing += Charged_Callback;
        _crossbow.OnShoot += Shoot_Callback;
    }

    void StartCharging_Callback()
    {
        _chargedCrossbowCharged = FmodAudioManager.Instance.CreateInstance(Sounds.CrossbowCharge);
        _chargedCrossbowCharged.start();
    }

    void Charged_Callback()
    {
        _chargedCrossbowCharged.stop(STOP_MODE.ALLOWFADEOUT);
        _chargedCrossbowCharged.release();
    }

    void Shoot_Callback()
    {
        FmodAudioManager.Instance.PlayOneShot(Sounds.CrossbowShoot2D);
        FmodAudioManager.Instance.PlayOneShotForOthersRPC(Sounds.CrossbowShoot3D, transform.position);
    }
}
