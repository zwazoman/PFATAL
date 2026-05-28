using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private void Start()
    {
        SceneManager.activeSceneChanged += SceneChanged_Callback;
    }

    void SceneChanged_Callback(Scene previousScene, Scene newScene)
    {
        if(newScene.name == "Map")
        {
            if(AudioManager.Instance.playSounds)
                AudioManager.Instance.PlayOneShot(Sounds.Music);
        }
            
    }
}
