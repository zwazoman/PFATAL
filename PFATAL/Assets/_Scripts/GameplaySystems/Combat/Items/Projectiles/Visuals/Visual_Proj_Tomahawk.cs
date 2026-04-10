using UnityEngine;

public class Visual_Proj_Tomahawk : Proj_Visual
{
    [SerializeField] float _spinSpeed = 750;

    protected override void Update()
    {
         visuals.transform.Rotate(_spinSpeed * Time.deltaTime, 0, 0);

        base.Update();
    }
}
