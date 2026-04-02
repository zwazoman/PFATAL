using UnityEngine;

public class Proj_WolfTrap : Projectile
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _duringTime = 25f;
    [SerializeField] float _throwStrength = 25f;
    [SerializeField] float _activationDelay = 0.5f;

    float _timer;
    bool _isArmed = false;

    private void Awake()
    {
        TryGetComponent(out _rb);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

        _timer = 0f;
        _rb.isKinematic = false;

        Vector3 force = transform.forward * 5 + transform.up * 3;
        _rb.AddForce(force.normalized * _throwStrength, ForceMode.Impulse);

        Invoke(nameof(ArmTrap), _activationDelay);
    }

    void ArmTrap()
    {
        _isArmed = true;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;
    }

    private void Update()
    {
        if (!IsSpawned || !IsServer) return;

        _timer += Time.deltaTime;

        if (_timer >= _duringTime)
        {
            Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isArmed || !IsServer) return;

        if (other.TryGetComponent(out DamageableObject hit))
        {
            Debug.Log("WolfTrap triggered");

            // futur state (freeze)

            Despawn();
        }
    }
}