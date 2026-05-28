using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class GolemLook : MonoBehaviour
{
    [SerializeField] Transform _golemHead;
    [SerializeField] Camera _cam;
    [SerializeField] float _spaceDistance = 1;

    private void LateUpdate()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _spaceDistance);
        Vector3 lookPoint = _cam.ScreenToWorldPoint(mousePos);

        Vector3 dir = (lookPoint - transform.position).normalized;

        // Rotation qui ferait regarder le forward vers la cible
        Quaternion targetRot = Quaternion.LookRotation(dir);

        // Compensation : le "bas" du modèle devient le forward logique
        _golemHead.rotation = targetRot * Quaternion.Euler(-90, 0, 0);
    }
}
