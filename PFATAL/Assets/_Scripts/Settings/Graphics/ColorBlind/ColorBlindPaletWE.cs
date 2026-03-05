using UnityEngine;
using UnityEditor;
using UnityEngine.Accessibility;

#if UNITY_EDITOR

/// <summary>
///  A window editor so that we can know what multiple symptoms of color blindness sees on a choosen color.
/// </summary>
public class ColorBlindPaletWE : EditorWindow
{
    // size of swatch background textures to generate
    private const int _swatchTextureSize = 16;
    // the maximum number of swatches for this example
    private const int _maxPaletteSize = 11;

    [MenuItem("Window/Color Swatch Example")]
    private static void CreateWindow()
    {
        var window = GetWindow<ColorBlindPaletWE>();
        window.position = new Rect(0f, 0f, 400f, 80f);
    }

    // the background textures to use for the swatches
    private Texture2D[] _swatchBackgrounds = new Texture2D[_maxPaletteSize];

    // the desired number of swatches
    [SerializeField]
    private int _paletteSize = 8;
    // the range of desired luminance values
    [SerializeField]
    private Vector2 _desiredLuminance = new Vector2(0.2f, 0.9f);
    // the colors obtained
    [SerializeField]
    private Color[] _palette;
    // the number of unique colors in the palette before they repeat
    [SerializeField]
    private int _numUniqueColors;

    // create swatch background textures when window first opens
    protected virtual void OnEnable()
    {
        titleContent = new GUIContent("Color Swatches");

        // create background swatches with different patterns for repeated colors
        _swatchBackgrounds[0] = CreateSwatchBackground(_swatchTextureSize, 0, 0);
        _swatchBackgrounds[1] = CreateSwatchBackground(_swatchTextureSize, 1, 4);
        _swatchBackgrounds[2] = CreateSwatchBackground(_swatchTextureSize, 1, 3);
        _swatchBackgrounds[3] = CreateSwatchBackground(_swatchTextureSize, 6, 1);
        _swatchBackgrounds[4] = CreateSwatchBackground(_swatchTextureSize, 4, 3);
        _swatchBackgrounds[5] = CreateSwatchBackground(_swatchTextureSize, 6, 6);
        _swatchBackgrounds[6] = CreateSwatchBackground(_swatchTextureSize, 4, 2);
        _swatchBackgrounds[7] = CreateSwatchBackground(_swatchTextureSize, 6, 4);
        _swatchBackgrounds[8] = CreateSwatchBackground(_swatchTextureSize, 2, 5);
        _swatchBackgrounds[9] = CreateSwatchBackground(_swatchTextureSize, 1, 2);

        UpdatePalette();
    }

    // clean up textures when window is closed
    protected virtual void OnDisable()
    {
        for (int i = 0, count = _swatchBackgrounds.Length; i < count; ++i)
            DestroyImmediate(_swatchBackgrounds[i]);
    }

    protected virtual void OnGUI()
    {
        // input desired number of colors and luminance values
        EditorGUI.BeginChangeCheck();

        _paletteSize = EditorGUILayout.IntSlider("Palette Size", _paletteSize, 1, _maxPaletteSize);

        float min = _desiredLuminance.x;
        float max = _desiredLuminance.y;
        EditorGUILayout.MinMaxSlider("Luminance Range", ref min, ref max, 0f, 1f);
        _desiredLuminance = new Vector2(min, max);

        if (EditorGUI.EndChangeCheck())
        {
            UpdatePalette();
        }

        // display warning message if parameters are out of range
        if (_numUniqueColors == 0)
        {
            string warningMessage = "Unable to generate any unique colors with the specified luminance requirements.";
            EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
        }
        // otherwise display swatches in a row
        else
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                Color oldColor = GUI.color;

                int swatchBackgroundIndex = 0;
                for (int i = 0; i < _paletteSize; ++i)
                {
                    // change swatch background pattern when reaching a repeated color
                    if (i > 0 && i % _numUniqueColors == 0)
                        ++swatchBackgroundIndex;

                    Rect rect = GUILayoutUtility.GetRect(_swatchTextureSize * 2, _swatchTextureSize * 2);
                    rect.width = _swatchTextureSize * 2;

                    GUI.color = _palette[i];
                    GUI.DrawTexture(rect, _swatchBackgrounds[swatchBackgroundIndex], ScaleMode.ScaleToFit, true);
                }

                GUI.color = oldColor;
                GUILayout.FlexibleSpace();
            }
        }
    }

    // create a white texture with some pixels discarded to make a pattern
    private Texture2D CreateSwatchBackground(int size, int discardPixelCount, int discardPixelStep)
    {
        var swatchBackground = new Texture2D(size, size);
        swatchBackground.hideFlags = HideFlags.HideAndDontSave;
        swatchBackground.filterMode = FilterMode.Point;

        var pixels = swatchBackground.GetPixels32();
        int counter = 0;
        bool discard = false;
        for (int i = 0, count = pixels.Length; i < count; ++i)
        {
            pixels[i] = new Color32(255, 255, 255, (byte)(discard ? 0 : 255));
            ++counter;
            if (discard && counter == discardPixelCount)
            {
                discard = false;
                counter = 0;
            }
            else if (!discard && counter == discardPixelStep)
            {
                discard = true;
                counter = 0;
            }
        }
        swatchBackground.SetPixels32(pixels);

        swatchBackground.Apply();
        return swatchBackground;
    }

    // request new palette
    private void UpdatePalette()
    {
        _palette = new Color[_paletteSize];
        _numUniqueColors =
            VisionUtility.GetColorBlindSafePalette(_palette, _desiredLuminance.x, _desiredLuminance.y);
    }
}
#endif
