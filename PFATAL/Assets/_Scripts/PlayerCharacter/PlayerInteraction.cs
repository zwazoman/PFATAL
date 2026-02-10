using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] public PlayerMain main;

    Interactable _currentInteractable;

    [Header("Parameters")]
    [SerializeField] float _interactionWidth;
    [SerializeField] float _interactionRange;

    [SerializeField] LayerMask _interactionmask;

    [HideInInspector] public bool canInteract;

    private void Update()
    {
        RaycastHit hit;

        if (Physics.SphereCast(main.playerCamera.transform.position, _interactionWidth, /*main.playerCamera.transform.position + */main.playerCamera.transform.forward, out hit, _interactionRange, _interactionmask))
        {
            if (hit.collider.gameObject.TryGetComponent(out Interactable interactable))
            {
                if(interactable != _currentInteractable &&_currentInteractable !=null)
                    _currentInteractable.StopHover();

                _currentInteractable = interactable;
                interactable.StartHover();
            }
        }
        else
        {
            if(_currentInteractable != null)
            {
                _currentInteractable.StopHover();
                _currentInteractable = null;
            }
        }
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            print("try interact");
            if(_currentInteractable != null)
                _currentInteractable.Interact(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(main.playerCamera.transform.position, main.playerCamera.transform.position + main.playerCamera.transform.forward.normalized * _interactionRange);
        Gizmos.DrawWireSphere(main.playerCamera.transform.position + main.playerCamera.transform.forward.normalized * _interactionRange, _interactionWidth);
    }
}
