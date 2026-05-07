using System;
using UnityEngine;

public class SplashAnimEventReceiver : MonoBehaviour
{
    public event Action OnAnimEnded;

    public void AnimEnded() => OnAnimEnded?.Invoke();
}
