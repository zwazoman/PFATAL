using Unity.Netcode;
using UnityEngine;

public class Interactable : NetworkBehaviour
{
    [HideInInspector] public bool isInteractable = true;

    [SerializeField] MeshRenderer _mR;
    [SerializeField] Material _initialMaterial;
    [SerializeField] Material _hoveredMaterial;

    protected virtual void Awake()
    {
        if (_mR = GetComponent<MeshRenderer>())
            _initialMaterial = _mR.material;

        if (gameObject.layer != 6)
            gameObject.layer = 6;
    }

    public virtual void Interact(PlayerInteraction interaction) { }

    public virtual void StartHover()
    {
        //feedback

        if(_hoveredMaterial != null)
            _mR.material = _hoveredMaterial;
    }

    public virtual void StopHover()
    {
        //feedback
        if(_initialMaterial != null)
            _mR.material = _initialMaterial;
    }

    //gérer feedback
}
