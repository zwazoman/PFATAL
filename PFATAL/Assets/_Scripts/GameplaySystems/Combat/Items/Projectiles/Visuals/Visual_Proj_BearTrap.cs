using Unity.Netcode;
using UnityEngine;

public class BearTrapVisuals : MonoBehaviour
{
    [SerializeField] Proj_WolfTrap _bearTrap;
    [SerializeField] Animator _animator;

    private void Start()
    {
        _bearTrap.OnTrapPlayer += CloseTrap;
    }

    [Rpc(SendTo.Everyone)]
    void CloseTrap()
    {
        _animator.enabled = true;
        _animator.ResetTrigger("Close");
        _animator.SetTrigger("Close");
    }
}
