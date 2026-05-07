using System;
using UnityEngine;

public class SplashAnimEventReceiver : MonoBehaviour
{
    public event Action OnAnimEnded;

    public void AnimEnded() => OnAnimEnded?.Invoke();

    public void GrabHat() => AudioManager.Instance.PlayOneShot(Sounds.Pickup);
}
