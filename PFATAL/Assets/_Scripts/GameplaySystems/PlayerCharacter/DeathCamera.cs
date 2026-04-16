using Unity.Cinemachine;
using UnityEngine;

public class DeathCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cameraDeath;
    [SerializeField] private Camera _cameraWorld;
    [SerializeField] private Camera _cameraHands;

    [Header("Paramètres")]
    [SerializeField] private float orbitSpeed  = 60f;
    [SerializeField] private float orbitHeight = 1.5f;
    [SerializeField] private float orbitRadius = 3f;

    private CinemachineOrbitalFollow _orbital;
    private bool _isDead = false;

    private void Awake()
    {
        _orbital = _cameraDeath.GetCinemachineComponent(CinemachineCore.Stage.Body)
            as CinemachineOrbitalFollow;

        _orbital.OrbitStyle           = CinemachineOrbitalFollow.OrbitStyles.Sphere;
        _orbital.Radius               = orbitRadius;
        _orbital.VerticalAxis.Value   = orbitHeight;
        _orbital.HorizontalAxis.Value = 0f;
        
        _orbital.VerticalAxis.Range   = new Vector2(orbitHeight, orbitHeight);

        _cameraDeath.Priority = 0;
    }

    private void Update()
    {
        if (!_isDead) return;

        _orbital.HorizontalAxis.Value += orbitSpeed * Time.deltaTime;
    }
    
    public void OnPlayerDeath()
    {
        _isDead = true;
        _cameraDeath.Priority = 1;
        _cameraDeath.gameObject.SetActive(true);
        
        _cameraWorld.enabled = false;
        _cameraHands.enabled = false;
    }

    public void OnPlayerRespawn()
    {
        _isDead = false;
        _cameraDeath.Priority = 0;
        _cameraDeath.gameObject.SetActive(false);
        
        _cameraWorld.enabled = true;
        _cameraHands.enabled = true;
    }
}
