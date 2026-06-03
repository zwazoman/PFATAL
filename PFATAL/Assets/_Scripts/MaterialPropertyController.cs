using UnityEngine;

public class MaterialPropertyController : MonoBehaviour
{
    [SerializeField] Renderer[] _characterRenderers;

    [Header("Properties")]
    [SerializeField] float _health = 1f;
    [Range(0f, 0.1f)]
    [SerializeField] float _emissiveTintIntensity = 0.1f;

    MaterialPropertyBlock _mpb;
    int _healthID;
    int _emissiveTintID;
    
    Color _baseEmissiveTint1;

    void Start()
    {
        _mpb = new MaterialPropertyBlock();
        _healthID        = Shader.PropertyToID("_Health");
        _emissiveTintID = Shader.PropertyToID("_EmissiveTint1");

        if (_characterRenderers.Length > 0)
        {
            var mat = _characterRenderers[0].sharedMaterial;
            _baseEmissiveTint1 = mat.GetColor("_EmissiveTint1");
        }
    }

    void Update()
    {
        foreach (var r in _characterRenderers)
        {
            r.GetPropertyBlock(_mpb);
            _mpb.SetFloat(_healthID, _health);
            _mpb.SetColor(_emissiveTintID, _baseEmissiveTint1 * _emissiveTintIntensity);
            r.SetPropertyBlock(_mpb);
        }
    }
}