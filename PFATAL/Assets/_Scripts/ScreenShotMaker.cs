using UnityEngine;

public class ScreenShotMaker : MonoBehaviour
{
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            int i = PlayerPrefs.GetInt("screenshotID", 0) ;
        
            string filename = "Assets/_Graph/ScreenShots/highResScreenShot_" + i.ToString() + ".png";
            ScreenCapture.CaptureScreenshot(filename, 2);

            PlayerPrefs.SetInt("screenshotID", i + 1);
        }
    }

#endif
}
