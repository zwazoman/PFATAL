using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] string _splashName = "Splash";
    [SerializeField] string _menuName = "MainMenu";
    [SerializeField] string _mapName = "Map";
    [SerializeField] string _podiumName = "PodiumScene";

    EventInstance _splashAmbienceInstance;

    private void Awake()
    {
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => SceneChanged_Callback(scene);
    }

    void SceneChanged_Callback(Scene newScene)
    {
        if (!AudioManager.Instance.playSounds) 
            return;

        if (newScene.name == _splashName)
        {
            _splashAmbienceInstance = AudioManager.Instance.CreateInstance(Sounds.SplashAmbience,false, false);
            _splashAmbienceInstance.start();
        }

        if(newScene.name == _menuName)
        {
            //todo => musique dans le menu
        }

        if (newScene.name == _mapName)
        {
            _splashAmbienceInstance.stop(STOP_MODE.ALLOWFADEOUT);
            _splashAmbienceInstance.release();

            AudioManager.Instance.PlayOneShot(Sounds.Music);
        }

    }
}
