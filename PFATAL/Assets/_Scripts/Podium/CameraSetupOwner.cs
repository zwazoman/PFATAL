using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using System.Collections;

public class CameraSetupOwner : NetworkBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private CinemachineCamera cam;

    [Header("Délai avant focus")]
    [SerializeField] private float delay = 1f;

    [Header("Zoom")]
    [SerializeField] private float startFOV = 70f;
    [SerializeField] private float targetFOV = 30f;
    [SerializeField] private float zoomDuration = 2f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public void SetTarget(Transform target)
    {
        StartCoroutine(FocusOnPlayer(target));
    }

    private IEnumerator FocusOnPlayer(Transform target)
    {
        yield return new WaitForSeconds(delay);

        cam.enabled = true;
        vcam.enabled = true;
        
        vcam.Follow = target;
        vcam.LookAt = target;

        StartCoroutine(ZoomIn());
    }

    private IEnumerator ZoomIn()
    {
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;

            float t = zoomCurve.Evaluate(elapsed / zoomDuration);
            
            var lens = vcam.Lens;
            lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            vcam.Lens = lens;

            yield return null;
        }
        
        var finalLens = vcam.Lens;
        finalLens.FieldOfView = targetFOV;
        vcam.Lens = finalLens;
    }
}