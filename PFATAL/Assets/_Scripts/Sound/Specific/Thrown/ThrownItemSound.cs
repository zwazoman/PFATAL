using UnityEngine;

public class ThrownItemSound<T> : ItemSound<T> where T : Cons_ThrowableBase
{
        
    protected override void EquipLink() => main.hand.animatorEventListener.OnObjectThrown += UseSound;

    protected override void UnEquipLink() => main.hand.animatorEventListener.OnObjectThrown -= UseSound;

    protected override void UseSound() => AudioManager.Instance.PlayOnlineOneShots(Sounds.TomahawkShoot, Sounds.TomahawkShoot3D, transform.position);

}
