using System;
using _Scripts.Pooling;
using UnityEngine;

public class ExplosionVisuals : MonoBehaviour
{
    [Header("scene references")]
    [SerializeField] private Explosion explosion;

    [Header("Parameters")] [SerializeField]
    private ExplosionVfxType _vfxType;
    enum ExplosionVfxType
    {
        big,
        small
    }
    
    void Awake()
    {
        explosion.EventOnExplode += OnExplode;
    }

    private void OnExplode(Vector3 position)
    {
        //pull vfx from pool
        try
        {
            PooledObject explosion = (_vfxType switch
            {
                ExplosionVfxType.big => LocalPoolManager.Instance.Pool_VFX_Explosion_big,
                ExplosionVfxType.small => LocalPoolManager.Instance.Pool_VFX_Explosion_small
            }).PullObjectFromPool(position, Quaternion.identity, null);

            if (_vfxType is ExplosionVfxType.big)
                explosion.transform.position += Vector3.up * -0.5f;

            explosion.GoBackIntoPool_Delayed(4);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
    }
}
