using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GammaSetter : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    private LiftGammaGain _gamma;

    private void Awake()
    {
        if (!_volume.profile.TryGet(out _gamma)) throw new System.NullReferenceException(nameof(_gamma));
    }
    
    public void SettingGamma(float gammaAlpha)
    {
        _gamma.gamma.Override(new Vector4(0, 0, 0, gammaAlpha));
    }
}
