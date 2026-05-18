using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : SoundComponent<Button>
{
    protected override void LinkEvents()
    {
        main.onClick.AddListener(() => AudioManager.Instance.PlayOneShot(Sounds.ButtonClick));
    }
}
