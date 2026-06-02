using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class LightCookieAnimation : MonoBehaviour
{
    [SerializeField] UniversalAdditionalLightData _light;
    [SerializeField] Vector2 scrolling;
    // Update is called once per frame
    void Update()
    {
        _light.lightCookieOffset = scrolling*Time.time;
    }
}
