using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private PlayerCharacterInputs _input;
    [SerializeField] private CharacterAiming _characterAiming;
    private bool _canDecrease = true;
    private RaycastHit _hitInfo;

    [SerializeField] private AimAssistVersions _assistVersions;

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
                    //_characterAiming.Sensitivity = 100;
                    _canDecrease = true;
                }
            }
        }
        else
        {
            //Will need some change after the change of sensitivity with the crossbow.
            //_characterAiming.Sensitivity = 100;
        }
    }

    private void Update()
    {
        if (_input.UsingGamePad == true)
        {
            if (Physics.SphereCast(_cameraTransform.position, 1f, _cameraTransform.forward, out _hitInfo, 100f, _mask))
            {
                AimAssist();
                /*
                if (_canDecrease == true)
                {
                    //_characterAiming.Sensitivity = 90;
                    _canDecrease = false;
                }*/
            }
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

            if (offset < _assistVersions.MaxOffset)
            {
                //Convert to local space.
                Vector3 localDifference = _cameraTransform.InverseTransformDirection(differenceToForward);
                localDifference /= _assistVersions.MaxOffset;

                //Aim assist is stronger the closer target is to the center.
                float strength = (_assistVersions.MaxOffset - offset) / _assistVersions.MaxOffset * _assistVersions.AssistStrength;
                localDifference *= strength;

                //Calculate InputMultiplier.
                float normalized = Mathf.Clamp01(offset / _assistVersions.MaxOffset);
                float inputMultiplier = Mathf.Lerp(1f, _assistVersions.MinInputMultiplier, 1f -  normalized);

                _characterAiming.AssistAim(localDifference, inputMultiplier);
            }
        }
    }
}
