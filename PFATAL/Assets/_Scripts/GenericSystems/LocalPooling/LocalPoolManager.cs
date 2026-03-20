using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Scripts.Pooling
{
    public class LocalPoolManager : MonoBehaviour
    {
        //singleton
        private static LocalPoolManager _instance;
        public static LocalPoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LocalPoolManager>();
                }
                return _instance;
            }
        }
    
        private void Awake()
        {
            _instance = this;
        }

        internal List<Pool> AllPools = new();

        [Header("VFXs")] 
        public Pool Pool_VFX_Hit_crit;
        public Pool Pool_VFX_Explosion_big;
        public Pool Pool_VFX_Explosion_small;

    }
}
