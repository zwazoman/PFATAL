using UnityEngine;

public abstract class SoundComponent<T> : MonoBehaviour where T : Component
{
    [Header("Main Ref")]
    [SerializeField] protected T main;

    virtual protected async void Awake()
    {
        TryGetComponent(out T newMain);
        if(newMain != null)
            main = newMain;

        while (AudioManager.Instance == null)
        {
            await Awaitable.NextFrameAsync();
        }

        if (AudioManager.Instance.playSounds)
            LinkEvents();
    }

    abstract protected void LinkEvents();
}
