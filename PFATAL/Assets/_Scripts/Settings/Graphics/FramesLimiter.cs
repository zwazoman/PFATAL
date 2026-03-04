using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class FramesLimiter : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _frameDropdown;
    private int _frames;

    private void Start()
    {
        FillDropdown();
    }

    public void FillDropdown()
    {
        _frames = 30;
        List<string> options = new List<string>();
        _frameDropdown.ClearOptions();
        for (int i = 0; i > 2; i++)
        {
            _frames = _frames * 2;
            options.Add(_frames.ToString());
        }
        _frameDropdown.AddOptions(options);
        //_frameDropdown.value = 0;
        _frameDropdown.RefreshShownValue();
    }

    public void SetLimitToFrames(int limit)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = limit;
    }
}
