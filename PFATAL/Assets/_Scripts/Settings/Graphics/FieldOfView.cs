using UnityEngine;

[ExecuteInEditMode]
public class FieldOfView : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    // I can't fucking do it because SOMEONE made it so that the FOV will ALWAYS STAY THE SAME INSIDE OF A LATE UPDATE!
    // (It's in PlayerCameraBehaviour for reference).
    public void FOVSet(float sliderAmount)
    {
        _camera.fieldOfView = sliderAmount;
    }
}
