using _scripts.PlayerCharacter;
using Unity.Netcode;
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
    void OnEnable()
    {
        _playerCharacter.health.OnHpChanged += OnHPChanged;
        _damageOverlayMaterial.SetFloat(NormalizedAmountProperty,0);
    }
    
    // Update is called once per frame
    void OnDisable()
    {
        _playerCharacter.health.OnHpChanged -= OnHPChanged;
        _damageOverlayMaterial.SetFloat(NormalizedAmountProperty,0);
    }

    void OnHPChanged(float h)
    {
        //client only code
        if (NetworkManager.Singleton.LocalClientId != _playerCharacter.OwnerClientId) return;
        float redAmount = 1.0f - (h / _playerCharacter.health.MaxHP);
        redAmount = redAmount * redAmount;
        redAmount = redAmount * .8f;
        _damageOverlayMaterial.SetFloat(NormalizedAmountProperty,redAmount);
    }
}
