using UnityEngine;

public class TutoPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
    }

    public void ShowPanel()
    {
        panel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ClosePanel()
    {
        panel.SetActive(false);

        Time.timeScale = 1f;
    }
}