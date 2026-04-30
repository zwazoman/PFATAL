using UnityEngine;
using UnityEngine.Events;

public class StartEvent : MonoBehaviour
{
    public UnityEvent unityEvent;
    void Start()
    {
        unityEvent.Invoke();
        Destroy(this);
    }
    
}
