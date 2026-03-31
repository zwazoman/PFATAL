using System;
using UnityEngine;

public class HammerEventReceiver : MonoBehaviour
{
    public event Action<bool> OnHitStart;
    public event Action OnHitEnd;

    public void StartHit(int dashedInt)
    {
        bool dashed = false;
        
        if(dashedInt == 1)
            dashed = true;
        
        OnHitStart?.Invoke(dashed);
    }

    public void EndHit() { OnHitEnd?.Invoke(); }
}
