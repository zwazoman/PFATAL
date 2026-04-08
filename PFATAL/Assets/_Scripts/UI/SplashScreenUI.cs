using UnityEngine;

public class SplashScreenUI : MonoBehaviour
{
    [SerializeField] SceneLoader _sceneLoader;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            _sceneLoader.LoadScene("MainMenu");
        }
    }
}
