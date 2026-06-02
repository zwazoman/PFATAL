using DG.Tweening;
using UnityEngine;

public class TutoPanel : MonoBehaviour
{
    public void ShowPanel()
    {
        gameObject.SetActive(true);
        transform.DOMoveX(200, 0.5f).SetEase(Ease.OutBack);
    }

    public void ClosePanel()
    {
        transform.DOMoveX(-500, 0.5f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
    }
}