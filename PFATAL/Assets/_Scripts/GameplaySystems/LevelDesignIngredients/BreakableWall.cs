using UnityEngine;

public class BreakableWall : IDamageable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(DamageData damageData)
    {
        Debug.Log("Breakable wall took damage: " + damageData.Amount);
    }
}
