using _scripts.PlayerCharacter;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScreenFeedbacks : MonoBehaviour
{
    private static readonly int NormalizedAmountProperty = Shader.PropertyToID("_normalizedAmount");

    [Header("Scene References")]
    [SerializeField] PlayerCharacter _playerCharacter;
    [Header("Asset References")]
    [SerializeField] Material _damageOverlayMaterial;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _playerCharacter.health.OnHpChanged += (float h) =>
        {
            float redAmount = 1.0f - (h / _playerCharacter.health.MaxHP);
            redAmount = redAmount * redAmount;
            redAmount = redAmount * .8f;
            _damageOverlayMaterial.SetFloat(NormalizedAmountProperty,redAmount);
        };
        _damageOverlayMaterial.SetFloat(NormalizedAmountProperty,0);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        _damageOverlayMaterial.SetFloat(NormalizedAmountProperty,0);
    }
}
