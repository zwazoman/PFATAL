using _Scripts.Extensions;
using _scripts.PlayerCharacter;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float globalMovespeedMultiplyer = 1;

    [Header("Scene References")]
    [SerializeField] private PlayerCharacter _playerCharacter;

    public void ApplyFriction(float friction, bool xzOnly)
    {
        Move(Vector3.zero,friction,xzOnly);
    }
    
    public void Move(Vector3 targetVelocity,float acceleration, bool xzOnly)
    {
        if (!enabled)
            return;

        Vector3 currentVelocity = _playerCharacter.physics.Velocity;
        Vector3 velocity = currentVelocity;
        
        if (xzOnly) {
            velocity.y = 0;
            targetVelocity.y = 0; }
        velocity = velocity.MoveToward(targetVelocity, acceleration);
        if (xzOnly) velocity.y = currentVelocity.y;
        //Debug.Log("old velocity : "+currentVelocity+", new velocity: "+velocity);
        _playerCharacter.physics.SetVelocity(velocity);
    }

    public async void TemporaryMoveSpeedChange(float speedMultiplyer, float duration)
    {
        globalMovespeedMultiplyer = speedMultiplyer;
        float t = 0;
        float _cameraFovOffset = 0;

        while (globalMovespeedMultiplyer != 1)
        {
             t += Time.deltaTime / duration;

            globalMovespeedMultiplyer = Mathf.Lerp(speedMultiplyer, 1, t);

            await Awaitable.NextFrameAsync();
        }
    }
    
}
