using TMPro;
using UnityEngine;

public class LoadingView : View
{
    [HideInInspector] public TMP_Text loadingText;

    public override void Activate()
    {
        base.Activate();

        if (loadingText.text == "")
            loadingText.text = "loading";
    }

    public override void Deactivate()
    {
        base.Deactivate();

        loadingText.text = "";
    }
}
