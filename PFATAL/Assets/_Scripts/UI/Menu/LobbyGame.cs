using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyGame : MonoBehaviour
{
    [SerializeField] Transform _golemHead;
    [SerializeField] Camera _cam;
    [SerializeField] float _spaceSize = 1;

    private void LateUpdate()
    {
        //Vector2 screenCenter = new Vector2(Screen.width/2, Screen.height/2);

        //Vector2 screenCenterToMoseDelta = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - screenCenter;

        //print(screenCenterToMoseDelta);

        //Vector3 LookPoint = _cam.transform.localPosition + new Vector3(screenCenterToMoseDelta.x, screenCenterToMoseDelta.y, 0) * _spaceSize;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _spaceSize);
        Vector3 lookPoint = _cam.ScreenToWorldPoint(mousePos);

        _golemHead.LookAt(lookPoint,_golemHead.forward);
        //_golemHead.rotation = Quaternion.Euler(_golemHead.rotation.x - 90, _golemHead.rotation.y, _golemHead.rotation.z);
        //_golemHead.rotation = Quaternion.AngleAxis(_golemHead.rotation.eulerAngles.x + 90, Vector3.right);
        //_golemHead.rotation = Quaternion.LookRotation(_golemHead.forward, -(lookPoint - _golemHead.position));
    }
}
