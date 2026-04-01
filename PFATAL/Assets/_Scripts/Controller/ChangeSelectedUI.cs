using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeSelectedUI : MonoBehaviour
{
    [SerializeField] private EventSystem _eventSystem;

    public void SwitchButtons(Selectable wichObject)
    {
        _eventSystem.SetSelectedGameObject(wichObject.gameObject);
    }
}
