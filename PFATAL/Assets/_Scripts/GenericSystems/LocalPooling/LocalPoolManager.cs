using System.Collections.Generic;
using UnityEngine;

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

        [Header("ui")]
        public Pool Pool_UI_DamageIndicator;
            
        [Header("VFXs")] 
        public Pool Pool_VFX_Hit_crit;
        public Pool Pool_VFX_Hit_Crossbow;
        public Pool Pool_VFX_Explosion_big;
        public Pool Pool_VFX_Explosion_small;
        public Pool Pool_VFX_GemBreak;
        public Pool Pool_VFX_GroundSlam;
        public Pool Pool_VFX_RockBurst_Small;
        public Pool Pool_VFX_RockBurst_Big;

    }
}
