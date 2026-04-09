using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private PlayerCharacterInputs _input;
    [SerializeField] private CharacterAiming _characterAiming;
    private bool _canDecrease = true;
    private RaycastHit _hitInfo;

    [Tooltip("The max tangent between your forward and your direction to the target.")]
    [Range(0f, 1f)][SerializeField] private float _maxOffset;
    [Tooltip("How much should the camera assist.")]
    [Range(0f, 1f)][SerializeField] private float _assistStrength;
    [Tooltip("How much the player can move the camera away.")]
    [Range(0f, 0.2f)][SerializeField] private float _minInputMultiplier;

    private void OnDrawGizmos()
    {
        if (_input.UsingGamePad == true)
        {
            //For seeing in debug mode if it works
            //Gizmos.DrawWireSphere(_cameraTransform.position, 100);

            if (Physics.SphereCast(_cameraTransform.position, 1f, _cameraTransform.forward, out _hitInfo, 100f, _mask))
            {
                //For seeing in debug mode if it works
                
                Gizmos.color = Color.green;
                Vector3 sphereCastMidpoint = transform.position + (transform.forward * _hitInfo.distance);
                Gizmos.DrawWireSphere(sphereCastMidpoint, 1f);
                Gizmos.DrawSphere(_hitInfo.point, 0.1f);
                Debug.DrawLine(transform.position, sphereCastMidpoint, Color.green);
                AimAssist();
                if (_canDecrease == true)
                {
                    //_characterAiming.Sensitivity = 90;
                    _canDecrease = false;
                }
            }
            else
            {
                //For seeing in debug mode if it works
                /*
                Gizmos.color = Color.red;
                Vector3 sphereCastMidpoint = transform.position + (transform.forward * (100f - 1f));
                Gizmos.DrawWireSphere(sphereCastMidpoint, 1f);
                Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);*/

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

    private void AimAssist()
    {
        foreach(RaycastHit target in Physics.SphereCastAll(_cameraTransform.position, 1f, _cameraTransform.forward, 100f, _mask))
        {
            //Get the direction of the target.
            Vector3 targetDirection = (target.point - _cameraTransform.position).normalized;

            //Determine difference from forward and store as offset.
            Vector3 differenceToForward = targetDirection - _cameraTransform.forward;
            float offset = differenceToForward.magnitude;

            if (offset < _maxOffset)
            {
                //Convert to local space.
                Vector3 localDifference = _cameraTransform.InverseTransformDirection(differenceToForward);
                localDifference /= _maxOffset;

                //Aim assist is stronger the closer target is to the center.
                float strength = (_maxOffset - offset) / _maxOffset * _assistStrength;
                localDifference *= strength;

                //Calculate InputMultiplier.
                float normalized = Mathf.Clamp01(offset / _maxOffset);
                float inputMultiplier = Mathf.Lerp(1f, _minInputMultiplier, 1f -  normalized);

                _characterAiming.AssistAim(localDifference, inputMultiplier);
            }
        }
    }
}
