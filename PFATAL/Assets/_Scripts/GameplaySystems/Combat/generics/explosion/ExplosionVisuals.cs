using SimpleVFXs;
using UnityEngine;

public class ExplosionVisuals : MonoBehaviour
{
    [Header("scene references")]
    private Explosion explosion;
    private StylisedEffect effect;
    void Awake()
    {
        explosion.EventOnExplode += OnExplode;
    }

    private void OnExplode()
    {
        effect.TriggerMainEvent();
    }
}
