using UnityEngine;

public abstract class SoundComponent<T> : MonoBehaviour where T : Component
{
    [Header("Main Ref")]
    [SerializeField] protected T main;

    virtual protected void Awake()
    {
        TryGetComponent(out T newMain);
        if(newMain != null)
            main = newMain;

        if (AudioManager.Instance.playSounds)
            LinkEvents();
    }

    abstract protected void LinkEvents();
}
