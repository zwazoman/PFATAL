using Unity.Netcode;
using UnityEngine;

public class BearTrapVisuals : NetworkBehaviour
{
    [SerializeField] Proj_WolfTrap _bearTrap;
    [SerializeField] Animator _animator;

    private void Start()
    {
        _bearTrap.OnTrapPlayer += CloseTrapRpc;
    }

    [Rpc(SendTo.Everyone)]
    void CloseTrapRpc()
    {
        _animator.enabled = true;
        _animator.ResetTrigger("Close");
        _animator.SetTrigger("Close");
    }
}
