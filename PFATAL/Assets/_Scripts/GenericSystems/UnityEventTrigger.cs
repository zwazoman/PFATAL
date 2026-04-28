using UnityEngine;
using UnityEngine.Events;

public class UnityEventTrigger : MonoBehaviour
{
    public UnityEvent unityEvent;
    
    public void TriggerEvent()
    {
        unityEvent.Invoke();
    }
}
