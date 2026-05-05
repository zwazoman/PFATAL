using UnityEngine;
using UnityEngine.Events;

public class StartEvent : MonoBehaviour
{
    public bool WaitOneFrame = true;
    public UnityEvent unityEvent;
    async void Start()
    {
        if(WaitOneFrame) await Awaitable.NextFrameAsync();
        unityEvent.Invoke();
        Destroy(this);
    }
    
}
