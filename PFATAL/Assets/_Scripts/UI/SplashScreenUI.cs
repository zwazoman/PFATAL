using DG.Tweening;
using TMPro;
using UnityEngine;

public class SplashScreenUI : MonoBehaviour
{
    [SerializeField] SceneLoader _sceneLoader;
    [SerializeField] GameObject _anim;
    [SerializeField] TMP_Text _text;
    [SerializeField] float _opacitySpeed = .3f;

    

    private void Start()
    {
        if(AudioManager.Instance.playSounds)
            AudioManager.Instance.PlayOneShot(Sounds.SplashAmbience);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_text.DOFade(0, _opacitySpeed));
        sequence.Append(_text.DOFade(1, _opacitySpeed)).SetEase(Ease.InCubic);
        sequence.SetLoops(-1);

        _anim.GetComponent<SplashAnimEventReceiver>().OnAnimEnded += AnimEndend_Callback;
    }

    void AnimEndend_Callback() => _sceneLoader.LoadScene("MainMenu");


    private void Update()
    {
        if (Input.anyKeyDown)
        {
            //_anim.GetComponent<Animation>().Play();
            _anim.GetComponent<Animator>().SetTrigger("GrabHat");
        }
    }
}
