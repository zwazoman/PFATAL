using UnityEngine;

public class SoundComponent<T> : MonoBehaviour where T : Component
{
    [Header("Main Ref")]
    [SerializeField] protected T main;

    virtual protected void Awake()
    {
        TryGetComponent(out T newMain);
        if(newMain != null)
            main = newMain;
    }

    private void Start()
    {
        if(AudioManager.Instance.playSounds)
            LinkEvents();
    }

    protected virtual void LinkEvents() { }
}
