using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    private RaycastHit _hitInfo;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private PlayerCharacterInputs _input;
    [SerializeField] private CharacterAiming _characterAiming;
    private bool _canDecrease = true;

    private void OnDrawGizmos()
    {
        if (_input.UsingGamePad == true)
        {
            //For seeing in debug mode if it works
            Gizmos.DrawWireSphere(_cameraTransform.position, 100);

            if (Physics.SphereCast(_cameraTransform.position, 1f, _cameraTransform.forward, out _hitInfo, 100f, _mask))
            {
                //For seeing in debug mode if it works
                Gizmos.color = Color.green;
                Vector3 sphereCastMidpoint = transform.position + (transform.forward * _hitInfo.distance);
                Gizmos.DrawWireSphere(sphereCastMidpoint, 1f);
                Gizmos.DrawSphere(_hitInfo.point, 0.1f);
                Debug.DrawLine(transform.position, sphereCastMidpoint, Color.green);

                if (_canDecrease == true)
                {
                    _characterAiming.Sensitivity = 90;
                    _canDecrease = false;
                }
            }
            else
            {
                //For seeing in debug mode if it works
                Gizmos.color = Color.red;
                Vector3 sphereCastMidpoint = transform.position + (transform.forward * (100f - 1f));
                Gizmos.DrawWireSphere(sphereCastMidpoint, 1f);
                Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);

                if (_canDecrease == false)
                {
                    _characterAiming.Sensitivity = 100;
                    _canDecrease = true;
                }
            }
        }
        else
        {
            //Will need some change after the change of sensitivity with the crossbow.
            _characterAiming.Sensitivity = 100;
        }
    }
}
