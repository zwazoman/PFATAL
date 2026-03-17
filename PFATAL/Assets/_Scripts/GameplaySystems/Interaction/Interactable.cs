using System;
using Unity.Netcode;
using UnityEngine;

public class Interactable : NetworkBehaviour
{
    public event Action OnInteract;
    public event Action OnStartHover;
    public event Action OnStopHover;

    [HideInInspector] public bool isInteractable = true;

    protected virtual void Awake()
    {
        if (gameObject.layer != 7)
            gameObject.layer = 7;
    }

    public virtual void Interact(PlayerInteraction interaction)
    {
        OnInteract?.Invoke();
    }

    public virtual void StartHover()
    {
        //feedback
        OnStartHover?.Invoke();
    }

    public virtual void StopHover()
    {
        //feedback
        OnStopHover?.Invoke();
    }
}
