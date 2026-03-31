using _scripts.PlayerCharacter;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private static Collider[] _colliderBuffer = new Collider[10];
    
    [SerializeField] public PlayerCharacter _playerCharacter;

    Interactable _currentInteractable;

    [Header("Parameters")]
    [SerializeField] float _interactionWidth;
    [SerializeField] float _interactionRange;

    [SerializeField] LayerMask _interactionmask;

    [HideInInspector] public bool canInteract;
    

    private void Update()
    {
        var size = Physics.OverlapCapsuleNonAlloc(
            _playerCharacter.playerCamera.transform.position, 
            _playerCharacter.playerCamera.transform.position + _playerCharacter.playerCamera.transform.forward * _interactionRange,
            _interactionWidth, _colliderBuffer, _interactionmask);

        if (size > 0)
        {
            for(int i =0;i<size;i++)
            {
                if (_colliderBuffer[i].gameObject.TryGetComponent(out Interactable interactable))
                {
                    if(interactable != _currentInteractable &&_currentInteractable !=null)
                        _currentInteractable.StopHover();

                    _currentInteractable = interactable;
                    interactable.StartHover();
                    break;
                }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!_playerCharacter.IsOwner)
            return;

        print(collision.gameObject.name);

        if (collision.gameObject.TryGetComponent(out Pickup _pickup))
            _pickup.Interact(this);
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if(_currentInteractable != null)
                _currentInteractable.Interact(this);
        }
    }
}
