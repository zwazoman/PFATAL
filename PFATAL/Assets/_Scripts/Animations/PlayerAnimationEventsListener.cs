using System;
using UnityEngine;

public class PlayerAnimationEventsListener : MonoBehaviour
{
    public event Action OnFootstep;

    public void SetFoot() { OnFootstep?.Invoke(); }
}
