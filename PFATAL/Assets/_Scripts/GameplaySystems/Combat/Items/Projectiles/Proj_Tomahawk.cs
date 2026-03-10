using UnityEngine;

public class Proj_Tomahawk : Proj_Falling
{
    [Header("Tomahawk")]
    [SerializeField] float _spinSpeed;

    public override void OnSpawn()
    {
        base.OnSpawn();
    }

    protected override void Update()
    {
        transform.Rotate(_spinSpeed * Time.deltaTime,0,0);

        base.Update();
    }
}
