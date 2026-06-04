using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Setting : MonoBehaviour
{
    [SerializeField] private SettingsValues _values;

    [Header("GameObjects")]
    [SerializeField] private Slider _sliderFOV;
    [SerializeField] private Slider _sliderSensitivity;
    [SerializeField] private TMP_Text _textMoveUp;
    [SerializeField] private TMP_Text _textMoveDown;
    [SerializeField] private TMP_Text _textMoveLeft;
    [SerializeField] private TMP_Text _textMoveRight;
    [SerializeField] private TMP_Text _textJump;
    [SerializeField] private TMP_Text _textInteract;
    [SerializeField] private TMP_Text _textDrop;
    [SerializeField] private Slider _sliderGamma;
    [SerializeField] private Dropdown _dropdownFrames;
    [SerializeField] private Toggle _toggleVSYnc;

    void Start()
    {
        _sliderFOV.value = _values.FOV;
        _sliderSensitivity.value = _values.SensitivityMouse;

        _textMoveUp.text = _values.MoveUp;
        _textMoveDown.text = _values.MoveDown;
        _textMoveLeft.text = _values.MoveLeft;
        _textMoveRight.text = _values.MoveRight;
        _textJump.text = _values.Jump;
        _textInteract.text = _values.Interact;
        _textDrop.text = _values.Drop;
        
        _sliderGamma.value = _values.Gamma;
        _dropdownFrames.value = _values.FramesRates;
        _toggleVSYnc.isOn = _values.VSync;
    }
}
