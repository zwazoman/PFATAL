using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class FramesLimiter : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _frameDropdown;
    [SerializeField] private TMP_Text _framesText;
    private List<int> _frameList = new List<int>() { 30, 60, 120, 240 };
    [SerializeField] private Toggle _vsyncToggle;

    private void Start()
    {
        FillDropdown();

        //To check if the player saved and turned off Vsync.
        //To not forget : when saving option is implemented, put the saved frame rate on.
        if (QualitySettings.vSyncCount == 0)
        {
            _vsyncToggle.isOn = false;
            _frameDropdown.interactable = true;
        }
        else
        {
            _vsyncToggle.isOn = true;
            _frameDropdown.interactable = false;
        }
    }

    public void FillDropdown()
    {
        List<string> options = new List<string>();
        _frameDropdown.ClearOptions();
        for (int i = 0; i < _frameList.Count; i++)
        {
            options.Add(_frameList[i].ToString());
        }
        _frameDropdown.AddOptions(options);
        _frameDropdown.value = 0;
        _frameDropdown.RefreshShownValue();
    }

    public void OnOffVsync()
    {
        if (QualitySettings.vSyncCount == 0)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 0;
            _vsyncToggle.isOn = true;
            _frameDropdown.interactable = false;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            _vsyncToggle.isOn = false;
            _frameDropdown.interactable = true;
        }
    }

    public void SetLimitToFrames(int limit)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _frameList[limit];
    }

    //Just to see if it does limit the FPS.
    private void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        _framesText.text = (int)fps + " FPS";
    }
}
