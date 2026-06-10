using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraSetupOwner : MonoBehaviour
{
    [Header("GameObject")]
    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private GameObject scoreBoard;

    [Header("Délai avant focus")]
    [SerializeField] private float delay = 1f;

    [Header("Zoom")]
    [SerializeField] private float startFOV = 70f;
    [SerializeField] private float targetFOV = 30f;
    [SerializeField] private float zoomDuration = 2f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public void SetTarget(Transform target, Transform playerCharacterParent, Transform namePlayerParent)
    {
        StartCoroutine(FocusOnPlayer(target, playerCharacterParent, namePlayerParent));
    }

    private IEnumerator FocusOnPlayer(Transform target, Transform playerCharacterParent, Transform namePlayerParent)
    {
        yield return new WaitForSeconds(delay);

        if (vcam == null)
        {
            yield break;
        }

        cam.enabled = false;
        vcam.enabled = true;

        vcam.Follow = target;
        vcam.LookAt = target;

        HideOthers(target, namePlayerParent);

        StartCoroutine(ZoomIn());
    }

    private void HideOthers(Transform target, Transform namePlayerParent)
    {
        int localIndex = -1;
        int i = 0;

        i = 0;
        foreach (Transform child in namePlayerParent)
        {
            if (i != localIndex)
            {
                child.gameObject.SetActive(false);
                child.GetComponent<GametagUI>().SetPlayerName("");
            }
            i++;
        }
    }

    private IEnumerator ZoomIn()
    {
        var lens = vcam.Lens;
        lens.FieldOfView = startFOV;
        vcam.Lens = lens;

        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = zoomCurve.Evaluate(elapsed / zoomDuration);

            var l = vcam.Lens;
            l.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            vcam.Lens = l;

            yield return null;
        }

        var finalLens = vcam.Lens;
        finalLens.FieldOfView = targetFOV;
        vcam.Lens = finalLens;

        scoreBoard.gameObject.SetActive(true);
    }
}