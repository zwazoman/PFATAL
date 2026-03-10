using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace SimpleVFXs
{
    public class AutoDestroy : MonoBehaviour
    {
        [Min(0)]
        [SerializeField] float Delay = 1;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, Delay);
        }


    }
}
