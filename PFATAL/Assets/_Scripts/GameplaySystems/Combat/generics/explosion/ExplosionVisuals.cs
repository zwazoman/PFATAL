using SimpleVFXs;
using UnityEngine;

public class ExplosionVisuals : MonoBehaviour
{
    [Header("scene references")]
    [SerializeField] private Explosion explosion;
    [SerializeField] private StylisedEffect effect;
    void Awake()
    {
        explosion.EventOnExplode += OnExplode;
    }

    private void OnExplode()
    {
        effect.TriggerMainEvent();
    }
}
