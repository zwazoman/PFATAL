using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace SimpleVFXs
{

    public class StylisedEffect : MonoBehaviour
    {
        [SerializeField] bool TriggerMainEventOnAwake = true;
        [SerializeField] UnityEvent MainEvent;
        [SerializeField] float _playRate =1f;

        VisualEffect vfx;

        private void Awake()
        {
            //prevents the vfx from always playing on awake
            if (TryGetComponent<VisualEffect>(out vfx))
            {
                vfx.Stop();
            }
            
            //plays the whole event on awake
            if(TriggerMainEventOnAwake)
            {
                TriggerMainEvent();
            }
        }


        public void TriggerMainEvent()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                MainEvent.Invoke();
            }
            else //In edit mode, tries to play every monobehaviour method listening for the event anyways
            {
                for (int i = 0; i < MainEvent.GetPersistentEventCount(); i++)
                {
                    if ((MonoBehaviour)MainEvent.GetPersistentTarget(i) == null) continue;
                    ((MonoBehaviour)MainEvent.GetPersistentTarget(i)).SendMessage(MainEvent.GetPersistentMethodName(i),SendMessageOptions.DontRequireReceiver);
                }
            }
#else
            MainEvent.Invoke();
#endif
        }


        public void playVFX()
        {
            if (vfx == null) TryGetComponent(out vfx);
            vfx.playRate = _playRate;
            vfx.Play();
        }

        public void DetachFromParent()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            transform.parent = null;
        }
        public void PointUp()
        {
            transform.rotation = Quaternion.identity;
        }

        public void DestroySelf(float delay)
        {
            Destroy(gameObject, delay);
        }
    }




}