using _scripts.PlayerCharacter;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private static Collider[] _interactionColliderBuffer = new Collider[10];
    private static Collider[] _contactColliderBuffer = new Collider[10];
    
    [SerializeField] public PlayerCharacter _playerCharacter;

    Interactable _currentInteractable;

    [Header("Parameters")]
    [SerializeField] float _interactionWidth;
    [SerializeField] float _interactionRange;

    [SerializeField] LayerMask _interactionmask;

    [HideInInspector] public bool canInteract;

    private void Start()
    {
        canInteract = true;
    }


    private void Update()
    {
        //interaction detection
        var interactionSize = Physics.OverlapCapsuleNonAlloc(
            _playerCharacter.playerCamera.transform.position, 
            _playerCharacter.playerCamera.transform.position + _playerCharacter.playerCamera.transform.forward * _interactionRange,
            _interactionWidth, _interactionColliderBuffer, _interactionmask);

        if (interactionSize > 0)
        {
            for(int i =0;i<interactionSize;i++)
            {
                if (_interactionColliderBuffer[i].gameObject.TryGetComponent(out Interactable interactable))
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

        print(canInteract);

        if (!canInteract)
            return;

        //Contact detection
        int contactSize = Physics.OverlapSphereNonAlloc(transform.position, 1.2f,_contactColliderBuffer, _interactionmask);


        if(contactSize > 0)
        {
            for (int i = 0; i < contactSize; i++)
            {
                print(_contactColliderBuffer[i].gameObject.name);
                if (_contactColliderBuffer[i].gameObject.TryGetComponent(out Pickup pickup))
                    pickup.Interact(this);
            }
        }
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
