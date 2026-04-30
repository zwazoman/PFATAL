using DG.Tweening;
using TMPro;
using UnityEngine;

public class SplashScreenUI : MonoBehaviour
{
    [SerializeField] SceneLoader _sceneLoader;
    [SerializeField] TMP_Text _text;
    [SerializeField] float _opacitySpeed = .3f;

    private void Start()
    {
        AudioManager.Instance.PlayOneShot(Sounds.SplashAmbience);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_text.DOFade(0, _opacitySpeed));
        sequence.Append(_text.DOFade(1, _opacitySpeed)).SetEase(Ease.InCubic);
        sequence.SetLoops(-1);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            _sceneLoader.LoadScene("MainMenu");
        }



    }
}
