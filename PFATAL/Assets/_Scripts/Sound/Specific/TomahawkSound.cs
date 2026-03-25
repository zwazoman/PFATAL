using UnityEngine;

public class TomahawkSound : MonoBehaviour
{
    [SerializeField] Tomahawk _tomahawk;

    private void Awake()
    {
        TryGetComponent(out _tomahawk);
    }

    private void Start()
    {
        _tomahawk.OnShoot += Shoot_Callback;
    }

    void Shoot_Callback()
    {
        AudioManager.Instance.PlayOnlineOneShots(Sounds.TomahawkShoot2D, Sounds.TomahawkShoot3D, transform.position, _tomahawk.playerCharacter.OwnerClientId);
    }
}
