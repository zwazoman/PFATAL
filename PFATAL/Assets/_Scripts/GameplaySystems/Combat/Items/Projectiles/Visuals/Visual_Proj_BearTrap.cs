using UnityEngine;

public class BearTrapVisuals : MonoBehaviour
{
    [SerializeField] Proj_WolfTrap _bearTrap;
    [SerializeField] Animator _animator;

    private void Start()
    {
        _bearTrap.OnTrapPlayer += CloseTrap;
    }

    void CloseTrap()
    {
        _animator.SetTrigger("Close");
    }
}
